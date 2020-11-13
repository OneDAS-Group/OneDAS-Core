$hostName = "http://localhost:8080"

# get token
$credentials = @{
    username = 'root@onedas.org';
    password = '#root0/User1'
}

$token = Invoke-RestMethod -Uri "$hostName/api/v1/account/token" `
                           -Method POST `
                           -Body ($credentials | ConvertTo-Json) `
                           -ContentType "application/json"

# go
$headers = @{
    Authorization = "Bearer $token"
}

$aggregationParameters = @{
    begin = (Get-Date).Date.AddDays(-2).ToString('o')
    end = (Get-Date).Date.AddDays(-1).ToString('o')
    force = $false
    aggregations = @(
        @{
            projectId = '/AIRPORT/AD8_PROTOTYPE/GENERAL_DAQ'
            method = 'Mean'
            argument = 'None'
            filters = @{
                '--include-group' = 'MetMast|PowerTransducer'
                '--exclude-unit' = 'deg'
            }
        }
    )
}

$response = Invoke-RestMethod -Uri "$hostName/api/v1/jobs/aggregation" `
                              -Method POST `
                              -Headers $headers `
                              -Body ($aggregationParameters | ConvertTo-Json -Depth 3) `
                              -ContentType "application/json"
                              