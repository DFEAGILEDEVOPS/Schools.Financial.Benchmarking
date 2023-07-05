import {SetStateAction, useState} from "react";
import {Button, Modal} from "@mantine/core";
interface IRemoveWarningProps {
    removeScenarioHandler: (idx: 0 | 1) => void
    idx: 0 | 1
}
interface ICloseButtonProps {
    setOpenedHandler: React.Dispatch<SetStateAction<boolean>>;
}
const SfbModalCloseButton = (props : ICloseButtonProps)=> {
    return (
        <Button
            variant="subtle"
            color="dark"
            radius="xs"
            compact
            onClick={() => props.setOpenedHandler(false)}
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
    )
}

export default function SfbSadRemoveWarning({removeScenarioHandler, idx}: IRemoveWarningProps) {
    const [opened, setOpened] = useState<boolean>(false);
    const resetHandler = () => {
        removeScenarioHandler(idx);
        setOpened(false);
    }
    return (
        <>
            <Modal
                opened={opened}
                onClose={()=> setOpened(false)}
                size="xl"
                withCloseButton={false}>
                <div className="modal-header">
                    <div className="sfb-modal__header">
                        <h2 className="govuk-heading-m">
                            Are you sure you want to reset the dashboard?
                        </h2>
                        <SfbModalCloseButton setOpenedHandler={setOpened} />
                    </div>
                </div>

                <div className="sfb-modal-body">
                    <p>You will lose any data you have added.</p>
                </div>
                <div className="sfb-modal--actions">
                    <button className="govuk-button" onClick={()=> resetHandler()}>Reset</button>
                    <button className="link-button" onClick={()=> setOpened(false)}>Cancel</button>
                </div>
            </Modal>
            <button
                className="govuk-button govuk-button--secondary"
                onClick={() => setOpened(true)}>Reset dashboard</button>

        </>
    )
}
