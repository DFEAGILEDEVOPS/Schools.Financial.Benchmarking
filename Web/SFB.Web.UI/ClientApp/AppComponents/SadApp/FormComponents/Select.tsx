import React from "react";
import {UseFormRegister} from "react-hook-form";
import {IFormValues} from "../Models/SadEditForm";
import SfbSadHelpModal from "../../Global/ModalComponents/SfbSadHelpModal";

declare var modalMap: any[];

const Select = React.forwardRef<
    HTMLSelectElement,
    { label: string, options: string[] } & ReturnType<UseFormRegister<IFormValues>>
>(({ onChange, onBlur, name, label, options }, ref) => {
    const modal = modalMap.filter((item) => item.FieldId === label);
    return (
        <div className="govuk-form-group">
            <div className="sfb-label-with-modal">
                <label className="govuk-label">{label}</label>
                {modal.length > 0 &&
                  <SfbSadHelpModal
                    modalTitle={modal[0].Title}
                    modalContent={modal[0].Content} />
                }
            </div>
            <select name={name} ref={ref} onChange={onChange} onBlur={onBlur} className="govuk-select govuk-input--width-20">
                {options.map((option) => {
                    return <option value={option} key={option}>{option}</option>
                })}
            </select>
        </div>
    );
});

export default Select;

