import {FieldErrors, Path, UseFormRegister, ValidationRule} from "react-hook-form";
import {IFormValues} from "../../Models/SadEditForm";
import React from "react";

interface IInputProps {
    label: Path<IFormValues>;
    register: UseFormRegister<IFormValues>;
    required: boolean | string;
    errors: FieldErrors<IFormValues>
    pattern?: ValidationRule<RegExp>;
    hint?: React.ReactNode;
    min?: ValidationRule<number>
}
export default function TextInput({register, label, required, errors, hint, pattern, min}: IInputProps) {
    const fieldHasError = typeof errors[label] !== 'undefined';
    const containerClassNames = fieldHasError ?
        "govuk-form-group govuk-form-group--error": "govuk-form-group";
    const inputClassNames = fieldHasError ?
        "govuk-input govuk-input--error": "govuk-input";

    return (
        <div className={containerClassNames}>
            <label className="govuk-label">
                {label}
            </label>
            {hint &&
                <div className="govuk-hint">{hint}</div>
            }
            {fieldHasError &&
                <div className="govuk-error-message">
                    <span className="govuk-visually-hidden">Error:</span>
                    {errors[label]?.message}
                </div>
            }
            <input
                className={inputClassNames}
                {...register(label, {
                    required,
                    pattern,
                    min,
                })}
                type="text" />
        </div>
    )
}
