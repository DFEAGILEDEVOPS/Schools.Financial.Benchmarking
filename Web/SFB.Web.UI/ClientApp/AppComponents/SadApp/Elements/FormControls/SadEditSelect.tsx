import React from "react";
import {UseFormRegister} from "react-hook-form";
import {IFormValues} from "../../Models/SadEditForm";

const Select = React.forwardRef<
    HTMLSelectElement,
    { label: string, options: string[] } & ReturnType<UseFormRegister<IFormValues>>
>(({ onChange, onBlur, name, label, options }, ref) => {
    return (
        <div className="govuk-form-group">
            <label className="govuk-label">{label}</label>
            <select name={name} ref={ref} onChange={onChange} onBlur={onBlur} className="govuk-select">
                {options.map((option) => {
                    return <option value={option} key={option}>{option}</option>
                })}
            </select>
        </div>
    );
});

export default Select;
