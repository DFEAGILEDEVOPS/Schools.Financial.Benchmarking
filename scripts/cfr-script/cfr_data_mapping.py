# input col is key
#  output col is value
rename_col_dict = {
    "LondonBorough": "London Borough", 
    "SchoolName": "School Name",
    "OverallPhase": "Overall Phase",
    "LowestAgeOfPupils": "Lowest age of pupils",
    "HighestAgeOfPupils": "Highest age of pupils",
    "IndividualPupilsFTE": "Full time equivalent number of pupils in school",
    "Ind_PC_FSM": "% of pupils eligible for FSM",
    "PeriodCoveredByReturn": "Period covered by return (months)",
    "DidNotSupplyFlag": "Did Not Supply flag", 
    "FederationYN": "Federation",
    "LeadSchool": "Lead school in federation",
    "IndTeachers_FTE": "FTE Number of teachers",
    "UrbanRural": "Urban /Rural",
    "LondonWeighting": "London Weighting",
    "Ind_PC_EHCP": "% of pupils with EHCP",
    "Ind_PC_SEN_Support": "% of pupils with SEN support",
    "PercentageOfPupilsWithEAL": "% of pupils with English as an additional language",
    "Ind_PC_Boarders": "% of pupils who are Boarders", 
    "AdmissionsPolicy": "Admissions policy", 
    "HasA6thForm": "Has a 6th form",
    "NoOfPupilsIn6thForm": "No of pupils in 6th form",
    "Pre16Funding": "I01 Funds delegated by the LA",
    "Post16Funding": "I02 Funding for 6th form students",
    "I01 2 Pre and Post-16 Funding": "I01/2 Total pre and Post-16 Funding", 
    "SEN": "I03 SEN funding", 
    "FundingForMinorityEthnicPupils": "I04 Funding for minority ethnic pupils", 
    "PupilPremium": "I05 Pupil Premium", 
    "OtherDfEEFARevenueGrants": "I06 Other government grants",
    "OtherIncomeLAandOtherGovernmentGrants": "I07 Other grants and payments",
    "IncomeFromFacilitiesAndServices": "I08a/b Total income from facilities and services", 
    "IncomeFromCatering": "I09 Income from catering", 
    "ReceiptsFromSupplyTeacherInsuranceClaims": "I10 Receipts from supply teacher insurance claims",
    "ReceiptsFromOtherInsuranceClaims": "I11 Receipts from other insurance claims", 
    "IncomeFromContributionsToVisitsEtc": "I12 Income from contributions to visits etc.",
    "DonationsAndOrVoluntaryFunds": "I13 Donations and/or private funds",
    "PupilFocussedExtendedSchoolFundingAndOrGrants": "I15 Pupil focussed extended school funding and / or grants",
    "CommunityFocussedSchoolFundingAndOrGrants": "I16 Community focussed school funding and / or grants",
    "CommunityFocusedSchoolFacilitiesIncome": "I17 Community focused school facilities income",
    "AdditionalGrantForSchools": "I18 Total additional grant for schools",
    "TeachingStaff": "E01 Teaching Staff", 
    "SupplyTeachingStaff": "E02 Supply teaching staff",
    "EducationSupportStaff": "E03 Education support staff", 
    "PremisesStaff": "E04 Premises staff", 
    "AdministrativeAndClericalStaff": "E05 Administrative and clerical staff", 
    "CateringStaff": "E06 Catering staff", 
    "OtherStaff": "E07 Cost of other staff",
    "IndirectEmployeeExpenses": "E08 Indirect employee expenses",
    "StaffDevelopmentAndTraining": "E09 Development and training",
    "SupplyTeacherInsurance": "E10 Supply teacher insurance",
    "StaffRelatedInsurance": "E11 Staff related insurance",
    "BuildingMaintenanceAndImprovement": "E12 Building maintenance and improvement",
    "GroundsMaintenanceAndImprovement": "E13 Grounds maintenance and improvement",
    "CleaningAndCaretaking": "E14 Cleaning and caretaking", 
    "WaterAndSewerage": "E15 Water and sewerage", 
    "Energy": "E16 Energy", 
    "Rates": "E17 Rates", 
    "OtherOccupationCosts": "E18 Other occupation costs", 
    "LearningResourcesNotICTEquipment": "E19 Learning resources (not ICT equipment)", 
    "ICTLearningResources": "E20 ICT learning resources", 
    "ExaminationFees": "E21 Exam fees", 
    "AdministrativeSuppliesNonEducational": "E22 Administrative supplies", 
    "OtherInsurancePremiums": "E23 Other insurance premiums", 
    "SpecialFacilities": "E24 Special facilities",
    "CateringSupplies": "E25 Catering supplies", 
    "AgencySupplyTeachingStaff": "E26 Agency supply teaching staff", 
    "EducationalConsultancy": "E27 Bought in professional services – curriculum", 
    "PFICharges": "E28b Bought in professional services, other, PFI", 
    "BoughtInProfessionalServicesOther": "E28a/b Total bought in professional services", 
    "InterestChargesForLoansAndBank": "E29 Loan interest", 
    "DirectRevenueFinancingRevenueContributionsToCapital": "E30 Direct revenue financing (revenue contributions to capital)", 
    "CommunityFocusedSchoolStaff": "E31 Community focused school staff", 
    "CommunityFocusedSchoolCosts": "E32 Community focused school costs", 
    "DirectRevenueFinancingCapitalReserveTransfers": "CI04 Direct revenue financing", 
    "RevenueReserve": "Revenue Reserve: B01 + B02 + B06",
    "InYearBalance": "In-year Balance: Total Income (I01:I18 - E30) - Total Expenditure (E01:E29 + E31 + E32)", 
    "GrantFunding": "Grant Funding: (I01:I07) + I15 + I16 + I18a/b/c/d", 
    "DirectGrant": "Direct Grants: I01:I02 + I06:I07", 
    "CommunityGrants": "Community Grants: I16+I18", 
    "TargetedGrants": "Targeted Grants: I03:I05 + I15", 
    "SelfGeneratedFunding": "Self Generated Funding: (I08a/b:I13) + I17", 
    "TotalIncome": "Total Income: I01:I18 - E30",
    "Teaching Staff": "E01 Teaching Staff: E01 Teaching Staff  E01",
    "SupplyStaff": "Supply Staff: E02 + E10 + E26 SupplyStaff",
    "Education support staff  E03": "Education support staff: E03 Education support staff  E03",
    "OtherStaffCosts": "Other Staff Costs: (E07:E09) + E11", 
    "StaffTotal": "Staff Total: (E01:E03) + E05 + (E07: E11) + E26", 
    "MaintenanceAndImprovement": "Maintenance & Improvement: E12 + E13 MaintenanceAndImprovement",
    "Premises": "Premises: (E12:E14) + E04 + E28b Premises",
    "CateringExp": "Catering Expenses: E06 + E25 CateringExp",
    "Occupation": "Occupation: E06 + (E15:E18) + E23 + E25",
    "SuppliesAndServices": "Supplies and Services: (E19:E22) + (E27:E28b)",
    "EducationalSupplies": "Educational Supplies: (E19:E21)", 
    "BroughtInProfessionalSevices": "Brought in Professional Sevices: (E27 + E28a)",
    "CommunityExp": "Community Exp: E31 + E32",
    "TotalExpenditure": "Total Expenditure: (E01:E29 + E31 + E32)"
}

columns_to_include = [
        "LA",
        "LA Name",
        "Region",
        "LondonBorough",
        "Estab",
        "LAEstab",
        "URN",
        "SchoolName",
        "Phase", 
        "OverallPhase",
        "LowestAgeOfPupils",
        "HighestAgeOfPupils",
        "Type", 
        "IndividualPupilsFTE",
        "Ind_PC_FSM", 
        "PeriodCoveredByReturn",
        "DidNotSupplyFlag",
        "FederationYN",
        "LeadSchool",
        "IndTeachers_FTE",
        "Gender",
        "UrbanRural",
        "LondonWeighting",
        "Ind_PC_EHCP",
        "Ind_PC_SEN_Support",
        "PercentageOfPupilsWithEAL",
        "Ind_PC_Boarders",
        "AdmissionsPolicy",
        "PFI",
        "HasA6thForm",
        "NoOfPupilsIn6thForm",
        "Pre16Funding",
        "Post16Funding",
        "I01 2 Pre and Post-16 Funding",
        "SEN",
        "FundingForMinorityEthnicPupils",
        "PupilPremium",
        "OtherDfEEFARevenueGrants",
        "OtherIncomeLAandOtherGovernmentGrants",
        "I08a Income from lettings",
        "I08b Income from facilities and services",
        "IncomeFromFacilitiesAndServices",
        "IncomeFromCatering",
        "ReceiptsFromSupplyTeacherInsuranceClaims",
        "ReceiptsFromOtherInsuranceClaims",
        "IncomeFromContributionsToVisitsEtc",
        "DonationsAndOrVoluntaryFunds",
        "PupilFocussedExtendedSchoolFundingAndOrGrants",
        "CommunityFocussedSchoolFundingAndOrGrants",
        "CommunityFocusedSchoolFacilitiesIncome",
        "I18a Income from the Coronavirus Job Retention Scheme",
        "I18b Income from the DfE grant scheme for reimbursing exceptional costs associated with COVID-19",
        "I18c Income from the Â£1bn COVID-19 catch-up package announced on 20 July 2020",
        "I18d Income from other additional grants",
        "AdditionalGrantForSchools",
        "TeachingStaff",
        "SupplyTeachingStaff",
        "EducationSupportStaff",
        "PremisesStaff",
        "AdministrativeAndClericalStaff",
        "CateringStaff",
        "OtherStaff",
        "IndirectEmployeeExpenses",
        "StaffDevelopmentAndTraining",
        "SupplyTeacherInsurance",
        "StaffRelatedInsurance",
        "BuildingMaintenanceAndImprovement",
        "GroundsMaintenanceAndImprovement",
        "CleaningAndCaretaking",
        "WaterAndSewerage",
        "Energy",
        "Rates",
        "OtherOccupationCosts",
        "LearningResourcesNotICTEquipment",
        "ICTLearningResources",
        "ExaminationFees",
        "AdministrativeSuppliesNonEducational",
        "OtherInsurancePremiums",
        "SpecialFacilities",
        "CateringSupplies",
        "AgencySupplyTeachingStaff",
        "EducationalConsultancy",
        "E28a Bought in professional services, other, not PFI",
        "PFICharges",
        "BoughtInProfessionalServicesOther",
        "InterestChargesForLoansAndBank",
        "DirectRevenueFinancingRevenueContributionsToCapital",
        "CommunityFocusedSchoolStaff",
        "CommunityFocusedSchoolCosts",
        "OB01 Opening pupil-focused revenue balance",
        "OB02 Opening community-focused revenue balance",
        "OB03 Opening capital balance",
        "CI01 Capital income",
        "CI03 Voluntary or private income",
        "DirectRevenueFinancingCapitalReserveTransfers",
        "CE01 Acquisition of land and existing buildings",
        "CE02 New construction, conversion and renovation",
        "CE03 Vehicles, plant, equipment and machinery",
        "CE04 Information and communication technology",
        "B01 Committed revenue balances",
        "B02 Uncommitted revenue balances",
        "B03 Devolved formula capital balance",
        "B05 Other capital balances",
        "B06 Community-focused school revenue balances",
        "B07 Outstanding balance on capital loans to school",
        "RevenueReserve",
        "InYearBalance",
        "GrantFunding",
        "DirectGrant",
        "CommunityGrants",
        "TargetedGrants",
        "SelfGeneratedFunding",
        "TotalIncome",
        "Teaching Staff  E01",
        "SupplyStaff",
        "Education support staff  E03",
        "OtherStaffCosts",
        "StaffTotal",
        "MaintenanceAndImprovement",
        "Premises",
        "CateringExp",
        "Occupation",
        "SuppliesAndServices",
        "EducationalSupplies",
        "BroughtInProfessionalSevices",
        "CommunityExp",
        "TotalExpenditure",
]