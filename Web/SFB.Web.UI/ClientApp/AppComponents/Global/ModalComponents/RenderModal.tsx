import React, {useState} from "react";
import {Modal} from "@mantine/core";
import HelpIcon from "./HelpIcon";
interface Props {
  modalTitle: string;
  modalContent: string;
  buttonId?: string;
}

export default function RenderModal(props: Props) {
  const [opened, setOpened] = useState(false);
  
  return (
   <>
    <Modal opened={opened}
      onClose={()=> setOpened(false)}
      title={props.modalTitle}>
      {props.modalContent}
    </Modal>
     <HelpIcon onClick={()=> setOpened(true)} buttonId={props.buttonId} />
   </>
  )
}