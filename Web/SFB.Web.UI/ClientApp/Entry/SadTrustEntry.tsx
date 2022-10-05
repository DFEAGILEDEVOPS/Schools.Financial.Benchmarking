import ReactDOM from 'react-dom/client';
import {BrowserRouter} from 'react-router-dom';
import SadTrustApp from '../AppComponents/SadTrustApp/SadTrustApp';
declare var baseName: string;

const el = document.getElementById('sad-trust-app');

if (el instanceof HTMLElement) {
  const root = ReactDOM.createRoot(el);
  root.render(<BrowserRouter basename={baseName}><SadTrustApp /></BrowserRouter>)
}
