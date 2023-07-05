import React, {useEffect, useState} from "react";
import { Route, Routes, useNavigate} from "react-router-dom";
import {IFormValues, SadDataObject} from "./Models/SadEditForm";
import {ITableRowData, prepRows} from "./helpers/prepRows";
import {prepDataRows} from "./helpers/SadDataPrep";
import {prepOutComes} from "./helpers/prepOutComes";
import {scenarioBuilder} from "./helpers/ScenarioBuilder";
import Default from "./Views/Default";
import CompareData from "./PageComponents/CompareData";
import EditForm from "./PageComponents/EditForm";
import {EstablishmentSadTableRow} from "./SadTable";

declare var initialData: SadDataObject;
declare var modalMap: any[];
let appLoaded = false;
export default function SadApp() {
  const navigate = useNavigate();
  const [
    estabData,
    setEstabData
  ] = useState<SadDataObject>(initialData);

  const [
    estabFormData,
    setEstabFormData
  ] = useState<IFormValues | null>(null);

  const [
    estabCustomData,
    setEstabCustomData
  ] = useState<SadDataObject | null>(null);

  const [
    estabCustomFormData,
    setEstabCustomFormData
  ] = useState<IFormValues | null>(null);

  const [
    scenarioRows,
    setScenarioRows
  ] = useState<ITableRowData | null>(null);

  const editStorageKey = `edit-${initialData.Urn}`;
  const customStorageKey = `custom-${initialData.Urn}`;
  const d = prepDataRows({...initialData});
  const outcomesData: EstablishmentSadTableRow[] = prepOutComes({...initialData});
  const removeScenario = (idx: 0 | 1) => {
    console.log('remove ', idx);
    if (idx === 0) {
      localStorage.removeItem(editStorageKey);
      setEstabData(initialData);
    }
    if (idx === 1) {
      localStorage.removeItem(customStorageKey);
      setEstabCustomFormData(null);
      setEstabCustomData(null);
    }
    navigate("/");
  }

  useEffect(() => {
    // edit > save
    if (appLoaded) {
      console.log('Effect , save??');
      if (estabFormData) {
        const scenario = scenarioBuilder(initialData, estabFormData);
        const d = prepDataRows(scenario);
        const rows = prepRows(d);
        console.log({scenario});
        scenario.lastEdit = new Date();
        setEstabData(scenario);
        setScenarioRows(rows);
        //setScenario0Label(estabFormData["Name of dashboard"])
        if (estabFormData?.StoreScenario) {
          localStorage.setItem(editStorageKey, JSON.stringify(rows));
        }
        if (estabCustomFormData) {
          navigate('/compare')
        }
        navigate("/")
      }
    }
  }, [estabFormData]);

  useEffect(() => {
    // custom > save
    if (appLoaded) {
      console.log('Effect , custom save??');
      if (estabCustomFormData) {
        const scenario = scenarioBuilder(initialData, estabCustomFormData);
        // const d = prepDataRows(scenario);
        // const rows = prepRows(d);
        scenario.lastEdit = new Date();
        setEstabCustomData(scenario);
        //setScenario1Label(estabCustomFormData["Name of dashboard"]);
        //setScenarioRows(rows);
        if (estabCustomFormData?.StoreScenario) {
          localStorage.setItem(customStorageKey, JSON.stringify(scenario));
        }
        navigate("/compare")
      }
    }
  }, [estabCustomFormData]);

  useEffect(() => {
    console.log("Effect , load");
    const localData = localStorage.getItem(editStorageKey);
    const customData = localStorage.getItem(customStorageKey);
    setEstabData(initialData);
    if (localData) {
      setScenarioRows(JSON.parse(localData));
    } else {
      setScenarioRows(prepRows(d));
    }
    console.log({customData});
    if (customData !== null) {
      console.log('oh?')
      const scenario = scenarioBuilder(estabData, JSON.parse(customData));
      console.log({scenario})
      setEstabCustomData(scenario);
      navigate('/compare');
    } else {
      setEstabCustomData(initialData);
    }
    appLoaded = true;

  }, [setScenarioRows]);

  return (
      <>
        <div className="govuk-width-container ">
          <main className="govuk-main-wrapper " id="main-content" role="main">
            <Routes>
              <Route
                  path={"/"}
                  element={
                    <Default
                        removeScenarioHandler={removeScenario}
                        estabCustomData={estabData}
                        scenarioRows={scenarioRows}
                        outcomesData={outcomesData}
                    />} />
              <Route
                  path={'/edit0'}
                  element={<EditForm
                      mode="edit"
                      estabData={estabData}
                      setData={setEstabFormData} />} />
              <Route
                  path={'/edit1'}
                  element={<EditForm
                      mode="create"
                      estabData={estabCustomData}
                      setData={setEstabCustomFormData} />} />

              <Route
                  path={'/compare'}
                  element={
                      <CompareData
                      scenario0={estabData}
                      scenario1={estabCustomData!}
                      outcomes={outcomesData}
                      removeScenarioHandler={removeScenario} />} />

              <Route path="*" element={<>404: Page Not Found </>}/>
            </Routes>
          </main>
        </div>
      </>
  );
}