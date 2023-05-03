import ReactDOM from "react-dom/client";

import SadApp from "../AppComponents/SadApp/SadApp";
import React from "react";
import {BrowserRouter} from "react-router-dom";
const el = document.getElementById('sad-app');
declare var baseName: string;

if (el instanceof HTMLElement) {
    const root = ReactDOM.createRoot(el);
    root.render(<BrowserRouter basename={baseName}><SadApp /></BrowserRouter>)
}
