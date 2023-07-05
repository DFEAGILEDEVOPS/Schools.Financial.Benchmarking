import {Dispatcher, IFormValues, numberWithCommas, SadDataObject} from "../Models/SadEditForm";
import {useNavigate} from "react-router-dom";
import {SubmitHandler, useForm} from "react-hook-form";
import {ErrorMessage} from "@hookform/error-message";
import TextInput from "../FormComponents/TextInput";
import Select from "../FormComponents/Select";
import SadEditCheckBox from "../FormComponents/SadEditCheckbox";
import SfbSadHeader from "./SfbSadHeader";
import SfbWarningMessage from "./SfbWarningMessage";


declare var initialData: SadDataObject;
const dashboardYears: string[] = initialData.AvailableScenarioTerms;

interface IEditFormProps {
    scenario?: number;
    mode: 'edit' | 'create' | 'createUpdate';
    estabData: SadDataObject | null;
    setData: Dispatcher<IFormValues>;
}

const populateAssessmentAreaValue = (areaName: string, scenarioData: SadDataObject| null): number| undefined => {
    if (scenarioData) {
        return scenarioData.SadAssesmentAreas.filter((area) => {
            return area.AssessmentAreaName === areaName
        })[0]?.SchoolDataLatestTerm;
    }

    return initialData.SadAssesmentAreas.filter((area)=>{
        return area.AssessmentAreaName === areaName
    })[0]?.SchoolDataLatestTerm;
}
export default function EditForm({mode, estabData, setData}: IEditFormProps){
    let vm: IFormValues
    const navigate = useNavigate();
    //if (mode === 'edit') {
    vm = {
        "Name of dashboard": estabData? `${estabData.LatestTerm} submitted data` : '',
        "Year of dashboard": estabData? estabData.LatestTerm : initialData.LatestTerm,
        "School workforce (FTE)": estabData? estabData.WorkforceTotalLastTerm : initialData.WorkforceTotalLastTerm,
        "Number of teachers (FTE)": estabData? estabData.TeachersTotalLastTerm: initialData.TeachersTotalLastTerm ,
        "Senior leadership (FTE)": estabData? estabData.TeachersLeaderLastTerm : initialData.TeachersLeaderLastTerm,
        "Number of pupils (FTE)": estabData? estabData.NumberOfPupilsLatestTerm: initialData.NumberOfPupilsLatestTerm,
        "Total income": estabData? estabData.TotalIncomeLatestTerm: initialData.TotalIncomeLatestTerm,
        "Total expenditure": estabData? estabData.TotalExpenditureLatestTerm: initialData.TotalExpenditureLatestTerm,
        "Revenue reserve": populateAssessmentAreaValue("Revenue reserve", estabData),
        "Spending on teaching staff": populateAssessmentAreaValue("Teaching staff", estabData),
        "Spending on supply staff": populateAssessmentAreaValue("Supply staff", estabData),
        "Spending on education support staff": populateAssessmentAreaValue("Education support staff", estabData),
        "Spending on administrative and clerical staff": populateAssessmentAreaValue("Administrative and clerical staff", estabData),
        "Spending on other staff costs": populateAssessmentAreaValue("Other staff costs", estabData),
        "Spending on premises costs": populateAssessmentAreaValue("Premises costs", estabData),
        "Spending on educational supplies": populateAssessmentAreaValue("Educational supplies", estabData),
        "Spending on energy": populateAssessmentAreaValue("Energy", estabData),
        "Teacher contact ratio (less than 1)": populateAssessmentAreaValue("Pupil to teacher ratio", estabData),
        "Predicted percentage pupil number change in 3-5 years": populateAssessmentAreaValue("Predicted percentage pupil number change in 3-5 years", estabData),
        "Average class size": populateAssessmentAreaValue("Average Class size", estabData),
        StoreScenario: true,
    }
    //}

    const {
        register,
        handleSubmit,
        watch,
        formState: { errors },
        getValues,
        setValue,
        setError,
        getFieldState,
    } = useForm<IFormValues>({
        // @ts-ignore
        defaultValues: vm,
        criteriaMode: 'all'
    });
    const validateTotalExpenditure = (value: string): boolean | string => {
        const expFields = [
            "Spending on teaching staff",
            "Spending on supply staff",
            "Spending on education support staff",
            "Spending on administrative and clerical staff",
            "Spending on other staff costs",
            "Spending on premises costs",
            "Spending on educational supplies",
            "Spending on energy"
        ];
        // @ts-ignore
        const expenditureFieldValues:number[] = getValues(expFields)?.map(val => Number(val));
        const totalSpend = expenditureFieldValues.reduce((subTotal, a) => subTotal + a, 0);
        const isValid = Number(value) >= totalSpend;

        if (!isValid) {
            expFields.forEach((fld) => {
                console.log(fld);
                const currentValue = getValues(fld as keyof IFormValues);
                setValue(fld  as keyof IFormValues, currentValue, { shouldValidate: true });

                setError(fld as keyof IFormValues, {
                    type: 'custom',
                    message: "Please check this value",
                })
                console.log(getFieldState(fld as keyof IFormValues));
            })
        }
        return isValid || `Please check the expenditure fields. The total of your entered values is £${numberWithCommas(totalSpend.toFixed(2))} `;
    };

    const validateTotalWorkforce = (value: string): boolean | string => {
        const teacherCount = getValues("Number of teachers (FTE)");
        return !(Number(value) < Number(teacherCount)) || "The total workforce cannot be less than the number of teachers (FTE)"
    }

    const validateMaxLeadership = (value: string): boolean | string => {
        const leadersCount = getValues("Senior leadership (FTE)");
        const hasError = !(Number(value) < Number(leadersCount));
        const message = "The total school leadership (FTE) cannot be more than the number of teachers (FTE)";
        if (hasError) {
            setError("Senior leadership (FTE)", {
                type: 'manual',
                message,
            });
        }
        return hasError || message;
    }

    const generateHint = (fieldName: keyof IFormValues, viewModel: IFormValues, formatCurrency?: boolean) => {
        const value = viewModel[fieldName];
        if (value) {
            if (formatCurrency) {
                return <>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(value.toString())}</strong></>
            }
            return <>The {vm["Year of dashboard"]} value was <strong>{value}</strong></>
        }
        return <>No value recorded for {vm["Year of dashboard"]}</>
    }
    const onSubmit: SubmitHandler<IFormValues> = (data) => {
        //console.log('submit ', data);
        console.log(JSON.stringify(errors, null, 2));
        if (Object.keys(errors).length > 0) {
            document.getElementById('error-summary')?.focus();
        } else {
            const typedData = {}
            for (const field in data) {
                if (field === "Name of dashboard" ||
                    field === "Year of dashboard" ||
                    field === "StoreScenario") {
                    // @ts-ignore
                    typedData[field] = data[field]
                } else {
                    // @ts-ignore
                    typedData[field] = Number(data[field]);
                }
            }

            console.log(JSON.stringify(typedData, null, 2));
            // @ts-ignore
            setData(typedData);
        }
    }
    return (
        <>
            <div className="govuk-grid-row">
                <div className="govuk-grid-column-full">
                    {Object.keys(errors).length > 0 &&
                      <div className="govuk-error-summary" id="error-summary">
                        <div role="alert">
                          <h2 className="govuk-error-summary__title">
                            There is a problem
                          </h2>
                          <div className="govuk-error-summary__body">
                            <ul className="govuk-list govuk-error-summary__list">
                                {Object.keys(errors).map((fieldName) => (
                                    <li key={fieldName}>
                                        <ErrorMessage
                                            onClick={(e:React.MouseEvent<HTMLElement>)=> {
                                                e.preventDefault();
                                                //@ts-ignore
                                                errors[fieldName].ref.focus()}
                                            }
                                            href="#"
                                            errors={errors}
                                            name={fieldName as any}
                                            as="a" />
                                    </li>
                                ))}
                            </ul>
                          </div>
                        </div>
                      </div>
                    }
                    <SfbSadHeader
                        heading={mode === 'edit'? "Edit your self-assessment dashboard" : "Add a custom dashboard"}
                        schoolName={initialData.Name}
                        urn={initialData.Urn}
                        dashboardYear={estabData? estabData.LatestTerm: initialData.LatestTerm}
                        suppressDataDescription={true}
                    />
                </div>
            </div>
            {mode === "create" &&
              <div className="govuk-grid-row">
                <div className="govuk-grid-column-full">
                  <SfbWarningMessage
                    message={`Custom dashboards are for personal use and only 
                    visible to you. Any changes you make will be viewable on 
                    subsequent visits to this school’s dashboard unless you choose to reset them.`} />
                </div>
              </div>
            }
            <form onSubmit={handleSubmit(onSubmit)}>
                <div className="govuk-grid-row">
                    <div className="govuk-grid-column-full">
                        <fieldset className="govuk-fieldset">
                            <legend className="govuk-fieldset__legend govuk-fieldset__legend--m">
                                Dashboard details
                            </legend>
                            <TextInput
                                label="Name of dashboard"
                                register={register}
                                required={"Please enter a dashboard name to continue"}
                                errors={errors}/>

                            <Select
                                label="Year of dashboard"
                                options={dashboardYears}
                                {...register("Year of dashboard")}
                                required={true} />
                        </fieldset>
                        <fieldset className="govuk-fieldset">
                            <legend className="govuk-fieldset__legend govuk-fieldset__legend--m">
                                School details
                            </legend>
                            <TextInput
                                label={"Number of pupils (FTE)"}
                                register={register}
                                required={"Enter the number of pupils to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Number of pupils (FTE)", vm)} />

                            <TextInput
                                label={"School workforce (FTE)"}
                                register={register}
                                required={"Enter the school workforce count to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                validate={{
                                    exceedsTeacherCount: validateTotalWorkforce,
                                }}
                                hint={generateHint("School workforce (FTE)", vm)} />

                            <TextInput
                                label={"Number of teachers (FTE)"}
                                register={register}
                                required={"Enter the number of teachers to continue "}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                validate={{
                                    schoolLeadersExceeded: validateMaxLeadership,
                                }}
                                hint={generateHint("Number of teachers (FTE)", vm)} />

                            <TextInput
                                label={"Senior leadership (FTE)"}
                                register={register}
                                required={"Enter the school leadership number to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Senior leadership (FTE)", vm)} />

                            <TextInput
                                label={"Teacher contact ratio (less than 1)"}
                                register={register}
                                required={false}
                                errors={errors}
                                pattern={{
                                    value: /^0?\.\d+$/,
                                    message: 'Please enter a number between 0 and 1',
                                }}
                            />

                            <TextInput
                                label={"Predicted percentage pupil number change in 3-5 years"}
                                register={register}
                                required={false}
                                errors={errors}
                                min={{
                                    value: -100,
                                    message: 'Please enter a value greater than -100'
                                }} />

                            <TextInput
                                label={"Average class size"}
                                register={register}
                                required={false}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }} />

                        </fieldset>
                        <fieldset className="govuk-fieldset">
                            <legend className="govuk-fieldset__legend govuk-fieldset__legend--m">
                                Totals
                            </legend>
                            <TextInput
                                label={"Total income"}
                                register={register}
                                required={"Enter the total income to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Total income", vm, true)} />

                            <TextInput
                                label={"Total expenditure"}
                                register={register}
                                required={"Enter the total expenditure to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                validate={{
                                    exceedsExp: validateTotalExpenditure,
                                }}

                                hint={generateHint("Total expenditure", vm, true)} />

                            <TextInput
                                label={"Revenue reserve"}
                                register={register}
                                required={"Enter the revenue reserve to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Revenue reserve", vm, true)} />

                        </fieldset>
                        <fieldset className="govuk-fieldset">
                            <legend className="govuk-fieldset__legend govuk-fieldset__legend--m">
                                Expenditure
                            </legend>
                            <TextInput
                                label={"Spending on teaching staff"}
                                register={register}
                                required={"Enter the teaching staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Spending on teaching staff", vm, true)} />

                            <TextInput
                                label={"Spending on supply staff"}
                                register={register}
                                required={"Enter the supply staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Spending on supply staff", vm, true)} />

                            <TextInput
                                label={"Spending on education support staff"}
                                register={register}
                                required={"Enter the education support staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Spending on education support staff", vm, true)} />

                            <TextInput
                                label={"Spending on administrative and clerical staff"}
                                register={register}
                                required={"Enter the administrative and clerical staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Spending on administrative and clerical staff", vm, true)} />

                            <TextInput
                                label={"Spending on other staff costs"}
                                register={register}
                                required={"Enter the other staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Spending on other staff costs", vm, true)} />

                            <TextInput
                                label={"Spending on premises costs"}
                                register={register}
                                required={"Enter the other premises spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Spending on premises costs", vm, true)} />

                            <TextInput
                                label={"Spending on educational supplies"}
                                register={register}
                                required={"Enter the other educational supplies spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Spending on educational supplies", vm, true)}/>

                            <TextInput
                                label={"Spending on energy"}
                                register={register}
                                required={"Enter the other energy spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^\d+(\.\d+)?$/,
                                    message: 'Please enter a number',
                                }}
                                hint={generateHint("Spending on energy", vm, true)} />
                        </fieldset>
                        <div className="govuk-form-group">
                            <SadEditCheckBox
                                label={"StoreScenario"}
                                register={register}
                                required={false} />
                        </div>
                        <button type="submit" className="govuk-button">Continue to dashboard</button>
                    </div>
                </div>
            </form>
        </>
    )
}
