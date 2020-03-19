function Initialize   

    import System.IO.*
    
    directoryPath   = fullfile(pwd, 'OneDas.DataManagement.Interface');
    fileNameSet     = Directory.GetFiles(directoryPath, '*.dll');
    fileNameSet     = cell(fileNameSet).';
    
    for fileName = fileNameSet.'
        try
            NET.addAssembly(char(fileName));
        catch
%             warning('Could not load ''%s\''.\n', char(fileName))
        end
    end
       
end