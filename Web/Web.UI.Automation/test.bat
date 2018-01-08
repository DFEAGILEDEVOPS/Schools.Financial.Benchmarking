
cd Web\Web.UI.Automation

bundle install --path vendor\bundle
bundle exec cucumber

if %ERRORLEVEL% gtr 0 exit 
echo “Automated tests completed successfully”