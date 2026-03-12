# Script to replace obsolete .NET MAUI methods with new async versions
# Run this from the solution root directory

$files = @(
    "Source\ViewModel\ProfilePageViewModel.cs",
    "Source\ViewModel\QRScannerPageViewModel.cs",
    "Source\ViewModel\FilterPageViewModel.cs",
    "Source\ViewModel\LocationDetailsPageViewModel.cs",
    "Source\ViewModel\AddCardDetailsPageViewModel.cs",
    "Source\View\HomePage.xaml.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "Processing $file..." -ForegroundColor Cyan
        
        $content = Get-Content $file -Raw
        
        # Replace DisplayAlert with DisplayAlertAsync
        $newContent = $content -replace 'DisplayAlert\(', 'DisplayAlertAsync('
        
        # Save if changed
        if ($content -ne $newContent) {
            Set-Content -Path $file -Value $newContent -NoNewline
            Write-Host "  ? Updated DisplayAlert calls" -ForegroundColor Green
        } else {
            Write-Host "  - No DisplayAlert calls found" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ? File not found: $file" -ForegroundColor Red
    }
}

Write-Host "`n? All files processed!" -ForegroundColor Green
Write-Host "Please rebuild the solution to verify all warnings are resolved." -ForegroundColor Cyan
