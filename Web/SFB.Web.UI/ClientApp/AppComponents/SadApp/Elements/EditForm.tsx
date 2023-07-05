import {SubmitHandler, useForm} from "react-hook-form";
import {IFormValues} from "../Models/SadEditForm";
import {ErrorMessage} from "@hookform/error-message";
import TextInput from "./FormControls/SadEditTextInput";
import Select from "./FormControls/SadEditSelect";
import React from "react";
import {SadDataObject} from "../../SadTrustApp/Models/sadTrustTablesModels";
import {numberWithCommas} from "../../Helpers/formatHelpers";

const dashboardYears: string[] = [
    "2020/2021",
    "2021/2022",
    "2022/2023",
    "2023/2024",
    "2024/2025"
];

interface IEditFormProps {
    scenario?: number;
    mode: 'edit' | 'create' | 'createUpdate';
    estabData: SadDataObject;
}
export default function EditForm({mode, estabData}: IEditFormProps){
    let vm: IFormValues 
    
    //if (mode === 'edit') {
        vm = {
            "Name of dashboard": `${estabData.LatestTerm} submitted data`,
            "Year of dashboard": estabData.LatestTerm,
            "School workforce (FTE)": estabData.WorkforceTotalLastTerm,
            "Number of teachers (FTE)": estabData.TeachersTotalLastTerm,
            "Senior leadership (FTE)": estabData.TeachersLeaderLastTerm,
            "Number of pupils (FTE)": estabData.NumberOfPupilsLatestTerm,
            "Total income": estabData.TotalIncomeLatestTerm,
            "Total expenditure": estabData.TotalExpenditureLatestTerm,
            "Revenue reserve": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName === "Revenue reserve")[0]?.SchoolDataLatestTerm,
            "Spending on teaching staff": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName === "Teaching staff")[0]?.SchoolDataLatestTerm,
            "Spending on supply staff": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName === "Supply staff")[0]?.SchoolDataLatestTerm,
            "Spending on education support staff": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName ==="Education support staff")[0]?.SchoolDataLatestTerm,
            "Spending on administrative and clerical staff": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName === "Administrative and clerical staff")[0]?.SchoolDataLatestTerm,
            "Spending on other staff costs": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName === "Other staff costs")[0]?.SchoolDataLatestTerm,
            "Spending on premises costs": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName === "Premises costs")[0]?.SchoolDataLatestTerm,
            "Spending on educational supplies": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName === "Educational supplies")[0]?.SchoolDataLatestTerm,
            "Spending on energy": estabData.SadAssesmentAreas.filter(
                (area) => area.AssessmentAreaName === "Energy")[0]?.SchoolDataLatestTerm,
        }
    //}
    
    const {
        register,
        handleSubmit,
        watch,
        formState: { errors }
    } = useForm<IFormValues>({
        // @ts-ignore
        defaultValues: vm 
    });
    const onSubmit: SubmitHandler<IFormValues> = (data) => {
        console.log('submit ', data);
        if (Object.keys(errors).length > 0) {
            document.getElementById('error-summary')?.focus();
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
                    <h1 className="govuk-heading-xl">Edit your self-assessment dashboard</h1>
                </div>
            </div>
            <form onSubmit={handleSubmit(onSubmit)}>
                <div className="govuk-grid-row">
                    <div className="govuk-grid-column-one-half">
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
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>{vm["Number of pupils (FTE)"]}</strong></>} />

                            <TextInput
                                label={"School workforce (FTE)"}
                                register={register}
                                required={"Enter the school workforce count to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>{vm["School workforce (FTE)"]}</strong></>} />

                            <TextInput
                                label={"Number of teachers (FTE)"}
                                register={register}
                                required={"Enter the number of teachers to continue "}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>{vm["Number of teachers (FTE)"]}</strong></>} />

                            <TextInput
                                label={"Senior leadership (FTE)"}
                                register={register}
                                required={"Enter the school leadership number to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>{vm["Senior leadership (FTE)"]}</strong></>} />

                            <TextInput
                                label={"Teacher contact ratio (less than 1)"}
                                register={register}
                                required={false}
                                errors={errors}
                                pattern={{
                                    value: /^(0(\.\d+)?|1(\.0+)?)$/,
                                    message: 'Please enter a number between 0 and 1',
                                }} />

                            <TextInput
                                label={"Predicted percentage pupil number change in 3-5 years"}
                                register={register}
                                required={false}
                                errors={errors}
                                min={{
                                    value: -100,
                                    message: 'Please enter a valur greater than -100'
                                }} />

                            <TextInput
                                label={"Average class size"}
                                register={register}
                                required={false}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
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
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Total income"].toString())}</strong></>} />

                            <TextInput
                                label={"Total expenditure"}
                                register={register}
                                required={"Enter the total expenditure to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Total expenditure"].toString())}</strong></>} />

                            <TextInput
                                label={"Revenue reserve"}
                                register={register}
                                required={"Enter the revenue reserve to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Revenue reserve"]?.toString())}</strong></>} />

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
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Spending on teaching staff"]?.toString())}</strong></>} />

                            <TextInput
                                label={"Spending on supply staff"}
                                register={register}
                                required={"Enter the supply staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Spending on supply staff"]?.toString())}</strong></>} />

                            <TextInput
                                label={"Spending on education support staff"}
                                register={register}
                                required={"Enter the education support staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Spending on education support staff"]?.toString())}</strong></>} />

                            <TextInput
                                label={"Spending on administrative and clerical staff"}
                                register={register}
                                required={"Enter the administrative and clerical staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Spending on administrative and clerical staff"]?.toString())}</strong></>} />

                            <TextInput
                                label={"Spending on other staff costs"}
                                register={register}
                                required={"Enter the other staff spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Spending on other staff costs"]?.toString())}</strong></>} />

                            <TextInput
                                label={"Spending on premises costs"}
                                register={register}
                                required={"Enter the other premises spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Spending on premises costs"]?.toString())}</strong></>} />
                            
                            <TextInput
                                label={"Spending on educational supplies"}
                                register={register}
                                required={"Enter the other educational supplies spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Spending on educational supplies"]?.toString())}</strong></>}/>

                            <TextInput
                                label={"Spending on energy"}
                                register={register}
                                required={"Enter the other energy spend to continue"}
                                errors={errors}
                                pattern={{
                                    value: /^[0-9]+$/,
                                    message: 'Please enter a number',
                                }}
                                hint={<>The {vm["Year of dashboard"]} value was <strong>£{numberWithCommas(vm["Spending on energy"]?.toString())}</strong></>} />
                        </fieldset>
                        <button type="submit" className="govuk-button">Continue to dashboard</button>
                    </div>
                </div>
            </form>
        </>
    )
}