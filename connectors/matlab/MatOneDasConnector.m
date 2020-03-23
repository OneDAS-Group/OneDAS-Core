classdef MatOneDasConnector
    
    properties
        Connector
    end
    
    methods
        
        function self = MatOneDasConnector(host, port, username, password)
            import OneDas.DataManagement.Connector.*
            
            if nargin == 2
                self.Connector = OneDasConnector(host, port, @(message) self.Print(message));    
            elseif nargin == 4
                self.Connector = OneDasConnector(host, port, @(message) self.Print(message), username, password);    
            else
                error('Two our four arguments must be passed.')
            end
        end
        
        function Export(self, dateTimeBegin, dateTimeEnd, fileFormat, fileGranularity, channels, targetDirectoryPath)
            
            % translate variable list into .NET string array
            netChannels = NET.createGeneric('System.Collections.Generic.List', {'System.String'});

            for netChannel = channels(:).'
                netChannels.Add(char(netChannel));
            end
            
            % export
            task = self.Connector.ExportAsync(dateTimeBegin, dateTimeEnd, fileFormat, fileGranularity, channels, targetDirectoryPath);
            self.AwaitTask(task)
        end
        
        function result = Load(self, dateTimeBegin, dateTimeEnd, channels)
            
            % translate variable list into .NET string array
            netChannels = NET.createGeneric('System.Collections.Generic.List', {'System.String'});

            for netChannel = channels(:).'
                netChannels.Add(char(netChannel));
            end
            
            % load
            task = self.Connector.LoadAsync(dateTimeBegin, dateTimeEnd, netChannels);
            self.AwaitTask(task)
            
            % process result         
            netResult = task.Result;
            result = containers.Map();
            
            for i = 1 : numel(channels)
                channel = char(channels(i));
                netChannelInfo = netResult.Item(channel);
                result(channel) = self.ConvertChannelInfo(netChannelInfo);
            end
        end
        
        function channelInfo = ConvertChannelInfo(self, netChannelInfo)
            channelInfo.Name                = char(netChannelInfo.Name);
            channelInfo.Group               = char(netChannelInfo.Group);
            channelInfo.Unit                = char(netChannelInfo.Unit);
            channelInfo.TransferFunctions   = self.ConvertTransferFunctions(netChannelInfo.TransferFunctions);
            channelInfo.DatasetName         = char(netChannelInfo.DatasetName);
            channelInfo.Values              = double(netChannelInfo.Values).';
        end
        
        function transferFunctions = ConvertTransferFunctions(~, netTransferFunctions)
            
            transferFunctions = struct([]);
            
            for i = 1 : netTransferFunctions.Count
                netTransferFunction = netTransferFunctions(i);
                
                transferFunction.DateTime   = netTransferFunction.DateTime;                
                transferFunction.Type       = char(netTransferFunction.Type);
                transferFunction.Option     = char(netTransferFunction.Option);
                transferFunction.Argument   = char(netTransferFunction.Argument);
                
                transferFunctions(i) = transferFunction;
            end
        end
        
        function AwaitTask(~, task)
            while (true)
                if (task.IsCompleted)

                    if (strcmp(task.Status, 'Faulted'))
                        fprintf('Failed. Reason: %s\n', char(task.Exception.InnerException.Message))
                    else
                        fprintf('Done.\n')
                    end

                    return
                else
                    pause(1)
                end
            end
        end
        
        function Print(~, message)
            message = strrep(char(message), '%', '%%');
            message = strrep(char(message), '\', '\\');
            fprintf(message)
        end
    end
end