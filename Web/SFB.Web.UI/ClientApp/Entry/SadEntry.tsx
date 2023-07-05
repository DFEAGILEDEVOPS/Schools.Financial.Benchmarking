import ReactDOM from "react-dom/client";

import SadApp from "../AppComponents/SadApp/SadApp";
const el = document.getElementById('sad-app');

if (el instanceof HTMLElement) {
    const root = ReactDOM.createRoot(el);
    root.render(<SadApp />)
}
