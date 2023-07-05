import {IFormValues} from '../Models/SadEditForm';
import {Path, UseFormRegister} from "react-hook-form";
import React from "react";

interface ICheckboxProps {
    label: Path<IFormValues>;
    register: UseFormRegister<IFormValues>
    required: boolean
}

export default function SadEditCheckBox(
    {
        label,
        register,
        required
    }: ICheckboxProps
) {

    return (
        <div className="govuk-checkboxes" data-module="govuk-checkboxes">
            <div className="govuk-checkboxes__item">
                <input
                    className="govuk-checkboxes__input"
                    type="checkbox"
                    id={label}
                    {...register(label, {required})} />
                <label className="govuk-label govuk-checkboxes__label" htmlFor={label}>
                    Allow your browser to store this data
                    (deselecting will remove the data when you end the session)
                </label>
            </div>
        </div>
    )
}
