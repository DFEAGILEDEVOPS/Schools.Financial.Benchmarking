#!/bin/bash

cd Web/Web.UI.Automation

/bin/bash --login -c "bundle install --path vendor/bundle"
/bin/bash --login -c "bundle exec cucumber BASE_URL='https://as-t1dv-sfb.azurewebsites.net'"
CUCUMBER_EXIT_CODE=$?

kill -9 $PID
exit $CUCUMBER_EXIT_CODE