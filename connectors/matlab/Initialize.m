function Initialize   

    import System.IO.*
    
    directoryPath   = fullfile(pwd, 'OneDas.DataManagement.Connector');
    fileNameSet     = Directory.GetFiles(directoryPath, '*.dll');
    fileNameSet     = cell(fileNameSet).';
    
    for fileName = fileNameSet.'
        try
            state = warning('off','all');
            NET.addAssembly(char(fileName));
        catch
            warning(state)
%             warning('Could not load ''%s\''.\n', char(fileName))
        end
    end
       
end