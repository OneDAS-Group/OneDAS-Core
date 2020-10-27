classdef OneDasConnector
    
    properties
        Scheme
        Host
        Port
        Username
        Password
        Connector
    end
    
    methods
        
        function self = OneDasConnector(scheme, host, port, username, password)
            if ~(nargin == 3 || nargin == 5)
                error('Three or five arguments must be passed.')
            end
            
            self.Scheme = scheme;
            self.Host = host;
            self.Port = port;
            self.Username = username;
            self.Password = password;
        end
        
        function Export(self, dateTimeBegin, dateTimeEnd, fileFormat, fileGranularity, channels, targetDirectoryPath)
            
            dateTimeBegin   = self.ToNetDateTime(dateTimeBegin);
            dateTimeEnd     = self.ToNetDateTime(dateTimeEnd);
            
            % translate channel list into .NET string array
            netChannels = NET.createGeneric('System.Collections.Generic.List', {'System.String'});

            for netChannel = channels(:).'
                netChannels.Add(char(netChannel));
            end
            
            % export
            task = self.Connector.ExportAsync(dateTimeBegin, dateTimeEnd, fileFormat, fileGranularity, netChannels, targetDirectoryPath);
            self.AwaitTask(task)
        end
        
        function result = Load(self, dateTimeBegin, dateTimeEnd, channelPaths)
            
            if (strlength(self.Username) > 0 && strlength(self.Password) > 0)
                token = self.GetToken();
            end
            
            result = containers.Map();
            
            for channelPath = channelPaths.'
                channelInfo = self.GetChannelInfo(channelPath, token);
                channelInfo.Values = self.GetData(dateTimeBegin, dateTimeEnd, channelPath, token);
                result(char(channelPath)) = channelInfo;
            end 
            
        end   
        
        function channelInfo = GetChannelInfo(self, channelPath, token)
            import matlab.net.*
            import matlab.net.http.*
            
            requestMessage = RequestMessage;
            requestMessage.Method = 'get';
            
            if (strlength(token) > 0)
                header = HeaderField;
                header.Name = "Authorization";
                header.Value = ['Bearer ' token];
                requestMessage.Header = header;
            end
            
            channelPathParts = split(channelPath, '/');
            projectId = urlencode(['/' char(channelPathParts(2)) '/' char(channelPathParts(3)) '/' char(channelPathParts(4))]);
            channelId = urlencode(char(channelPathParts(5)));
            
            uri = URI([self.Scheme '://' self.Host ':' num2str(self.Port) '/api/v1' ...
                '/projects/' projectId ...
                '/channels/' channelId]);
            
            response = send(requestMessage, uri);
            
            if (response.StatusCode ~= 200)
                error([num2str(response.StatusLine.StatusCode) ': ' char(response.StatusLine.ReasonPhrase)])
            end
            
            channelInfo = self.ToPascalCase(response.Body.Data);
        end
               
        function data = GetData(self, dateTimeBegin, dateTimeEnd, channelPath, token)
            import matlab.net.*
            import matlab.net.http.*
            
            requestMessage = RequestMessage;
            requestMessage.Method = 'get';
            
            if (strlength(token) > 0)
                header = HeaderField;
                header.Name = "Authorization";
                header.Value = ['Bearer ' token];
                requestMessage.Header = header;
            end
            
            channelPathParts = split(channelPath, '/');
            projectId = urlencode(['/' char(channelPathParts(2)) '/' char(channelPathParts(3)) '/' char(channelPathParts(4))]);
            channelId = urlencode(char(channelPathParts(5)));
            datasetId = urlencode(char(channelPathParts(6)));
            dateTimeBegin = datestr(dateTimeBegin, 'yyyy-mm-ddTHH:MM:SSZ');
            dateTimeEnd = datestr(dateTimeEnd, 'yyyy-mm-ddTHH:MM:SSZ');
            
            uri = URI([self.Scheme '://' self.Host ':' num2str(self.Port) '/api/v1/data' ...
                '?projectId=' projectId ...
                '&channelId=' channelId ...
                '&datasetId=' datasetId ...
                '&begin=' dateTimeBegin ...
                '&end=' dateTimeEnd]);
            
            response = send(requestMessage, uri);
            
           if (response.StatusCode ~= 200)
                error([num2str(response.StatusLine.StatusCode) ': ' char(response.StatusLine.ReasonPhrase)])
            end
            
            data = typecast(response.Body.Data, 'double');
        end
        
        function token = GetToken(self)
            import matlab.net.*
            import matlab.net.http.*
            
            requestMessage = RequestMessage;
            requestMessage.Method = 'post';
            messageBody = MessageBody;
            messageBody.Data = ['{"username": "' self.Username '", "password": "' self.Password '"}'];
            requestMessage.Body = messageBody;
            
            uri = URI([self.Scheme '://' self.Host ':' num2str(self.Port) '/identity/account/generatetoken']);
            
            response = send(requestMessage, uri);
            
            if (response.StatusCode ~= 200)
                error([num2str(response.StatusLine.StatusCode) ': ' char(response.StatusLine.ReasonPhrase)])
            end
                
            token = response.Body.Data;
        end
        
        function newStruct = ToPascalCase(~, struct)            
            for fieldName = fieldnames(struct).'
                oldFieldName = char(fieldName);
                newFieldName = [upper(oldFieldName(1)) oldFieldName(2 : end)];
            	newStruct.(newFieldName) = struct.(char(fieldName));              
            end
        end
    end
end