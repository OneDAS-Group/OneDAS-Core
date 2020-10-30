classdef OneDasConnector < matlab.net.http.ProgressMonitor
    
    properties
        Scheme
        Host
        Port
        Username
        Password
        Token
        
        % Progress monitor
        Direction
        Value
        ProgressAction
        LastProgress
    end
    
    methods
        
        function self = OneDasConnector(scheme, host, port, username, password)
            
            if ~(nargin == 3 || nargin == 5)
                error('Three or five arguments must be passed.')
            end
            
            self.Scheme     = scheme;
            self.Host       = host;
            self.Port       = port;
            self.Username   = username;
            self.Password   = password;
        end
               
        function result = Load(self, dateTimeBegin, dateTimeEnd, params)
            
            dateTimeBegin.TimeZone  = 'Z';
            dateTimeEnd.TimeZone    = 'Z';
            
            params.Begin            = datestr(dateTimeBegin, 'yyyy-mm-ddTHH:MM:ssZ');
            params.End              = datestr(dateTimeEnd, 'yyyy-mm-ddTHH:MM:ssZ');
                      
            result = containers.Map();
            
            try
                fprintf('Streaming ... \n');
                index = 0;
                
                for channelPath = params.ChannelPaths.'
                    channelInfo                 = self.GetChannelInfo(channelPath);
                    channelInfo.Values          = self.GetDataStream(...
                        params, channelPath, index, length(params.ChannelPaths));
                    result(char(channelPath))   = channelInfo;
                    index                       = index + 1;
                end 

                fprintf('Streaming ... Done.\n');
                
            catch ex
                error('Streaming ... Fail. Reason: %s\n', ex.message);
            end
            
        end
        
        function Export(self, dateTimeBegin, dateTimeEnd, params, targetFolder)
            
            dateTimeBegin.TimeZone  = 'Z';
            dateTimeEnd.TimeZone    = 'Z';
            
            params.Begin            = datestr(dateTimeBegin, 'yyyy-mm-ddTHH:MM:ssZ');
            params.End              = datestr(dateTimeEnd, 'yyyy-mm-ddTHH:MM:ssZ');
            
            self.GetDataFiles(params, targetFolder);
            
        end
        
        % Progress Monitor        
        function set.Value(self, value)
            self.Value = value;
            
            if (isempty(self.LastProgress))
                self.LastProgress = datetime;
            end
            
            if ((datetime - self.LastProgress) > duration(0, 0, 1))
                self.ProgressAction()
                self.LastProgress = datetime;
            end
        end
        
        function done(~)
            % do nothing
        end
        
    end
    
    methods (Access = private)
        
        function channelInfo = GetChannelInfo(self, channelPath)
            import matlab.net.*
            import matlab.net.http.*
            
            requestMessage          = RequestMessage;
            requestMessage.Method   = 'get';
            channelPathParts        = split(channelPath, '/');
            projectId               = urlencode(['/' char(channelPathParts(2)) '/' char(channelPathParts(3)) '/' char(channelPathParts(4))]);
            channelId               = urlencode(char(channelPathParts(5)));
            
            uri = URI([self.Scheme '://' self.Host ':' num2str(self.Port) '/api/v1' ...
                '/projects/' projectId ...
                '/channels/' channelId]);
            
            response                = self.Send(requestMessage, uri);          
            channelInfo             = self.ToPascalCase(response.Body.Data);
        end
                     
        function data = GetDataStream(self, params, channelPath, current, total)
            import matlab.net.*
            import matlab.net.http.*
            
            requestMessage          = RequestMessage;
            requestMessage.Method   = 'get';
            channelPathParts        = split(channelPath, '/');
            projectId               = urlencode(['/' char(channelPathParts(2)) '/' char(channelPathParts(3)) '/' char(channelPathParts(4))]);
            channelId               = urlencode(char(channelPathParts(5)));
            datasetId               = urlencode(char(channelPathParts(6)));
            
            uri = URI([self.Scheme '://' self.Host ':' num2str(self.Port) '/api/v1/data' ...
                '?projectId=' projectId ...
                '&channelId=' channelId ...
                '&datasetId=' datasetId ...
                '&begin=' params.Begin ...
                '&end=' params.End]);
            
            self.ProgressAction = @() fprintf('Streaming ... %3.0f %%\n', ...
                (double(self.Value) / double(self.Max) + current) / total * 100);
            
            options = matlab.net.http.HTTPOptions(...
                'ProgressMonitorFcn', @() self,...
                'UseProgressMonitor', true);
            
            response    = self.Send(requestMessage, uri, options);            
            data        = typecast(response.Body.Data, 'double');
        end
        
        function GetDataFiles(self, params, targetFolder)
            import matlab.net.*
            import matlab.net.http.*
            
            requestMessage          = RequestMessage;
            requestMessage.Method   = 'post';            
            requestMessage.Body     = params;
            uri                     = URI([self.Scheme '://' ...
                                           self.Host ':' ...
                                           num2str(self.Port) ...
                                           '/api/v1/jobs/export']);
            response                = self.Send(requestMessage, uri);
            updateUrl               = self.FindLocationHeader(response.Header).Value;
            
            while (true)
                
                jobStatus = self.GetJobStatus(URI(updateUrl));
                jobStatus = self.ToPascalCase(jobStatus);
                
                if (strcmp(jobStatus.Status, 'RanToCompletion'))
                    downloadUrl = jobStatus.Result;
                    break
                    
                elseif (strcmp(jobStatus.Status, 'Canceled'))
                    error('The job has been cancelled.')
                    
                elseif (strcmp(jobStatus.Status, 'Faulted'))
                    error(['The job is faulted. Reason: ' jobStatus.ExceptionMessage])
                    
                elseif (~isempty(jobStatus.ProgressMessage))
                    fprintf('%3.0f %%: %s\n', jobStatus.Progress * 100, jobStatus.ProgressMessage)
                end
                
                pause(0.5)
                
            end
            
            % download
            downloadFilePath = tempname;
            fprintf('Downloading ... ');
            websave(downloadFilePath, downloadUrl);
            fprintf('Done.\n');
            
            % unzip
            fprintf('Unzipping ... ');
            unzip(downloadFilePath, targetFolder);
            fprintf('Done.\n');
        end
        
        function jobStatus = GetJobStatus(self, uri)
            import matlab.net.http.*
            
            requestMessage          = RequestMessage;
            requestMessage.Method   = 'get';
            response                = self.Send(requestMessage, uri);
            jobStatus               = response.Body.Data;
        end
        
        function token = GetToken(self)
            import matlab.net.*
            import matlab.net.http.*
            
            requestMessage              = RequestMessage;
            requestMessage.Method       = 'post';
            header                      = HeaderField;
            header.Name                 = 'Content-Type';
            header.Value                = 'application/json';
            requestMessage.Header       = header;
            messageBody                 = MessageBody;
            
            messageBody.Data.Username   = self.Username;
            messageBody.Data.Password   = self.Password;
            requestMessage.Body         = messageBody;
            
            uri         = URI([self.Scheme '://' ...
                               self.Host ':' ...
                               num2str(self.Port) ...
                               '/api/v1/account/token']);
                           
            response    = send(requestMessage, uri); 
            self.EnsureSuccessStatusCode(response)

            token       = response.Body.Data;
        end
       
        function response = Send(self, requestMessage, uri, options)
            import matlab.net.http.*
            
            if (strlength(self.Username) > 0)
                
                % get token if there is no cached one
                if (isempty(self.Token))
                    self.Token = self.GetToken();
                end
                
                header                  = HeaderField;
                header.Name             = 'Authorization';
                header.Value            = ['Bearer ' self.Token];
                requestMessage.Header   = header;
                
                if (nargin == 3)
                    response         	= send(requestMessage, uri);
                else
                    response          	= send(requestMessage, uri, options);
                end
                
                % get new token if current one is not valid anymore
                if (response.StatusCode == 401)
                    self.Token              = self.GetToken();
                    header.Value            = ['Bearer ' self.Token];
                    requestMessage.Header   = header;
                    
                    if (nargin == 3)
                        response         	= send(requestMessage, uri);
                    else
                        response          	= send(requestMessage, uri, options);
                    end
                end
                                
            else
                response = send(requestMessage, uri);
            end
            
            self.EnsureSuccessStatusCode(response)
        end
        
        function EnsureSuccessStatusCode(~, response)
            if (~(200 <= response.StatusCode && response.StatusCode < 300))
                error([num2str(response.StatusLine.StatusCode) ': ' ...
                    char(response.StatusLine.ReasonPhrase) ' - ' ...
                    char(response.Body.Data)])
            end
        end
        
        function locationHeader = FindLocationHeader(~, headers)
            
            for header = headers
                if (strcmp(header.Name, 'Location'))
                    locationHeader = header;
                    return
                end
            end
            
            error('Location header could not be found.')
        end
     
        function newStruct = ToPascalCase(~, struct)
            for fieldName               = fieldnames(struct).'
                oldFieldName            = char(fieldName);
                newFieldName            = [upper(oldFieldName(1)) oldFieldName(2 : end)];
            	newStruct.(newFieldName)= struct.(char(fieldName));              
            end
        end
    end
end