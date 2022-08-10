import {initAll} from 'govuk-frontend';
/*import DfeUtils from '../ApplicationModules/DfeUtils';
import cookiePolicy from '../ApplicationModules/cookiePolicy';
import ComparisonList from '../ApplicationModules/ComparisonList';*/

import CookiePolicy from '../AppModules/cookiePolicy';

CookiePolicy.managePreferencesUi();
CookiePolicy.bannerActions();
CookiePolicy.manageRecruitmentNotification();
initAll();

