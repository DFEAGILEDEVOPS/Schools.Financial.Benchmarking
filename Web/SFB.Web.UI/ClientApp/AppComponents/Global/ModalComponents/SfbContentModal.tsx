import React, {useState} from 'react';
import {Modal, Button} from '@mantine/core';

interface Props {
  modalTitle?: string;
  modalContent?: string;
  useExclaimIcon?: boolean;
}

export default function SfbContentModal(props: Props) {
  const [opened, setOpened] = useState(false);

  return (
    <>
      <Modal opened={opened}
             onClose={() => setOpened(false)}
             withCloseButton={false}
             overflow="inside"
             size="xl">
        <div className="modal-header">
          <div className="sfb-modal__header">
            <h2 className="govuk-heading-m">{props.modalTitle}</h2>
            <Button
              variant="subtle"
              color="dark"
              radius="xs"
              compact
              onClick={() => setOpened(false)}
              styles={() => ({
                root: {
                  backgroundColor: '#fff',
                  border: 0,
                  height: 42,
                  paddingLeft: 10,
                  paddingRight: 10,
                  fontWeight: 'normal',
                  '&:hover': {
                    backgroundColor: '#fff',
                  },
                  '&:focus': {
                    backgroundColor: '#ffdd00'
                  }
                }
              })}
            >Close</Button>
          </div>
        </div>
        <div className="sfb-modal-body">
            {props.modalContent &&
              <div dangerouslySetInnerHTML={{__html: props.modalContent}}/>}
          </div>
          

      </Modal>
      <Button onClick={() => setOpened(true)}
              radius="xl"
              size="md"
              compact className="sfb-help-icon"
              title={`More about ${props.modalTitle}`}>
        {props.useExclaimIcon ? "!" : "?"}
      </Button>
    </>
  )
}