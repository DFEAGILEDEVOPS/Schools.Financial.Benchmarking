import ReactDOM from 'react-dom/client';
import SadTrustApp from '../AppComponents/SadTrustApp/SadTrustApp';

const el = document.getElementById('sad-trust-app');


if (el instanceof HTMLElement) {
  const root = ReactDOM.createRoot(el);
  root.render(<SadTrustApp />)
}
