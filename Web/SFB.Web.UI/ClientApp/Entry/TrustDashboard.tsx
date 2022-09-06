import ReactDOM from 'react-dom/client';
import Dashboard from '../AppComponents/SadTrustSummaryApp/Dashboard';
import SfbContentModal from '../AppComponents/Global/ModalComponents/SfbContentModal';

declare var uid: any;

const el = document.getElementById('sad-dashboard-app');
if (el instanceof HTMLElement) {
  const root = ReactDOM.createRoot(el);
  root.render(<Dashboard uid={uid} />)
}


const modalPlaceHolder = document.getElementById('modal-revenuereserve');
const modalTitle = "Revenue reserve";
const modalContent = modalPlaceHolder?.dataset.modalText;

if (modalPlaceHolder instanceof HTMLElement) {
  const modalRoot = ReactDOM.createRoot(modalPlaceHolder);
  modalRoot.render(<SfbContentModal modalTitle={modalTitle} modalContent={modalContent} />);
}