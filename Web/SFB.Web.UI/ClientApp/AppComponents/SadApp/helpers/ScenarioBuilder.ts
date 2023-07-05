import {SadAssessmentArea, SadDataObject} from "../Models/SadEditForm";
import {IFormValues} from "../Models/SadEditForm";
export const scenarioBuilder = (
    defaultData:SadDataObject,
    scenario: IFormValues
): SadDataObject => {
    // @ts-ignore
    const assessmentAreas: SadAssessmentArea[] = defaultData.SadAssesmentAreas.map((area) => {
        switch (area.AssessmentAreaName){
            case "Teaching staff":
                return {...area, SchoolDataLatestTerm: scenario["Spending on teaching staff"]};

            case "Supply staff":
                return {...area, SchoolDataLatestTerm: scenario["Spending on supply staff"]};

            case "Education support staff":
                return {...area, SchoolDataLatestTerm: scenario["Spending on education support staff"]};

            case "Administrative and clerical staff":
                return {...area, SchoolDataLatestTerm: scenario["Spending on administrative and clerical staff"]};

            case "Other staff costs":
                return {...area, SchoolDataLatestTerm: scenario["Spending on other staff costs"]};

            case "Premises costs":
                return {...area, SchoolDataLatestTerm: scenario["Spending on premises costs"]};

            case "Educational supplies":
                return {...area, SchoolDataLatestTerm: scenario["Spending on educational supplies"]};

            case "Energy":
                return {...area, SchoolDataLatestTerm: scenario["Spending on energy"]};

            case "In-year balance":
                const balance = scenario["Total income"] - scenario["Total expenditure"];
                return {...area, SchoolDataLatestTerm: balance};

            case "Revenue reserve":
                return {...area, SchoolDataLatestTerm: scenario["Revenue reserve"]};

            case "Average teacher cost":
                const avgCost = scenario["Spending on teaching staff"]! / scenario["Number of teachers (FTE)"];
                return {...area, SchoolDataLatestTerm: avgCost};

            case "Senior leaders as a percentage of workforce":
                const seniorLeaders = scenario["Senior leadership (FTE)"] / scenario["School workforce (FTE)"] * 100;
                return {...area, SchoolDataLatestTerm: seniorLeaders};

            case "Pupil to teacher ratio":
                const ptoTratio = scenario["Teacher contact ratio (less than 1)"] ? 
                    scenario["Teacher contact ratio (less than 1)"]: 0;
                return {...area, SchoolDataLatestTerm: ptoTratio};

            case "Pupil to adult ratio":
                const ptoAratio = scenario["Number of pupils (FTE)"] / scenario["School workforce (FTE)"]
                return {...area, SchoolDataLatestTerm: ptoAratio};

            case "Teacher contact ratio (less than 1)":
                return {...area, SchoolDataLatestTerm: scenario["Teacher contact ratio (less than 1)"]};

            case "Predicted percentage pupil number change in 3-5 years":
                return {...area, SchoolDataLatestTerm: scenario["Predicted percentage pupil number change in 3-5 years"]};

            case "Average Class size":
                return {...area, SchoolDataLatestTerm: scenario["Average class size"]};

            default:
                console.log(area.AssessmentAreaName);
                break;
        }
    });
    //console.log(JSON.stringify(assessmentAreas, null, 2));
    
    return {
        ...defaultData,
        TotalExpenditureLatestTerm: scenario["Total expenditure"],
        TotalIncomeLatestTerm: scenario["Total income"],
        TeachersTotalLastTerm: scenario["Number of teachers (FTE)"]!,
        NumberOfPupilsLatestTerm: scenario["Number of pupils (FTE)"],
        WorkforceTotalLastTerm: scenario["School workforce (FTE)"],
        SadAssesmentAreas: assessmentAreas,
        lastEdit: new Date(),
        userLabel: scenario["Name of dashboard"],
    };
}
