import {IFormValues} from "../Models/SadEditForm";
import {FieldErrors, Path, UseFormRegister, ValidationRule} from "react-hook-form";
import SfbSadHelpModal from "../../Global/ModalComponents/SfbSadHelpModal";

declare var modalMap: any[];
interface IInputProps {
    label: Path<IFormValues>;
    register: UseFormRegister<IFormValues>;
    required: boolean | string;
    errors: FieldErrors<IFormValues>
    pattern?: ValidationRule<RegExp>;
    hint?: React.ReactNode;
    min?: ValidationRule<number>
    max?: ValidationRule<number>
    validate? : any
}
export default function TextInput(
    {
        register,
        label,
        required,
        errors,
        hint,
        pattern,
        min,
        max,
        validate,
    }: IInputProps) {
    const fieldHasError = typeof errors[label] !== 'undefined';
    const containerClassNames = fieldHasError ?
        "govuk-form-group govuk-form-group--error": "govuk-form-group";
    const inputClassNames = fieldHasError ?
        "govuk-input govuk-input--width-20 govuk-input--error": "govuk-input govuk-input--width-20";
    const itemModal = modalMap.filter((modal) => modal.FieldId === label);
    const inputId = label.toLowerCase()
        .replaceAll(' ', '-')
        .replaceAll('(','')
        .replaceAll(')','');
    if(fieldHasError) {
        console.log(label, ' ', errors[label]?.message)
    }
    return (
        <div className={containerClassNames}>
            <div className="sfb-label-with-modal">
                <label className="govuk-label" htmlFor={inputId}>
                    {label}
                </label>
                {itemModal.length > 0 &&
                  <SfbSadHelpModal
                    modalTitle={itemModal[0].Title}
                    modalContent={itemModal[0].Content} />
                }
            </div>
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
                id={inputId}
                {...register(label, {
                    required,
                    pattern,
                    min,
                    max,
                    validate,
                })}
                type="text" />
        </div>
    )
}
