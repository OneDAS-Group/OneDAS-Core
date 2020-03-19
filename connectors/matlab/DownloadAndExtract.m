function DownloadAndExtract(hostName, port, targetDirectoryPath, settings)

    import System.*
    import OneDas.DataManagement.Interface.*
    
    % instantiate downloader, connect to server and download data
    downloader = Downloader(hostName, port, @(message) Print(message));
    
    try
        downloader.Connect();
    catch
        error(['Could not connect to server at ' hostName ':' num2str(port) '.'])
    end
    
    task = downloader.DownloadAndExtract(settings, targetDirectoryPath);
%     try
%         
%     catch ex
%         error(['Download failed. Reason: ' char(ex.ExceptionObject.Message)])
%     end
    
    % wait until task is completed, then return
    while (true)
        if (task.IsCompleted)
            downloader.Disconnect();
            
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