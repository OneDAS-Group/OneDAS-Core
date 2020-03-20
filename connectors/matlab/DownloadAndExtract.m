%%%%%%%%%% TODO: TEST ALSO WITHOUT AUTH %%%%%%%%%%%%%
function DownloadAndExtract(hostName, port, targetDirectoryPath, settings)

    import System.*
    import OneDas.DataManagement.Connector.*
    
    % instantiate downloader, connect to server and download data
    connector = OneDasConnector(hostName, port, @(message) Print(message), "test@root.org", "#test0/User1");
       
    task = connector.ExportAsync(settings, targetDirectoryPath);
%     try
%         
%     catch ex
%         error(['Download failed. Reason: ' char(ex.ExceptionObject.Message)])
%     end
    
    % wait until task is completed, then return
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

function Print(message)
    message = strrep(char(message), '%', '%%');
    message = strrep(char(message), '\', '\\');
    fprintf(message)
end