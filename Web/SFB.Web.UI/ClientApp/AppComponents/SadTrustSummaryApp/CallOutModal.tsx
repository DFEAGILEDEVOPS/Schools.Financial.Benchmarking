import React, {useState} from 'react';
import {Modal, Button, Group} from '@mantine/core';
import {IThreshold} from './Dashboard';
import {numberWithCommas} from "../Helpers/formatHelpers";

interface Props {
  schools: { name: string, categoryRatings: IThreshold[] }[]
  modalTitle: string,
  category: string
}

export default function CallOutModal({schools, modalTitle, category}: Props) {
  const [opened, setOpened] = useState<boolean>(false);

  return (
    <>
      <Modal
        opened={opened}
        onClose={() => setOpened(false)}
        withCloseButton={false}
        size="xl"
      >
        <div className="modal-header">
          <div className="sfb-modal__header">
            <h2 className="govuk-heading-m">{modalTitle}</h2>
            <Button
              variant="subtle"
              color="dark"
              radius="xs"
              compact
              onClick={() => setOpened(false)}
              styles={() => ({
                root: {
                  backgroundColor: '#fff',
                  border: 0,
                  height: 42,
                  paddingLeft: 10,
                  paddingRight: 10,
                  fontWeight: 'normal',
                  '&:hover': {
                    backgroundColor: '#fff',
                  },
                  '&:focus': {
                    backgroundColor: '#ffdd00'
                  }
                }
              })}
            >Close</Button>
          </div>
        </div>
        <div className="sfb-modal-body">
          <table className="govuk-table sfb-sadtrust-table ">
            <thead className="govuk-table__head">
            <tr className="govuk-table__row">
              <th className="govuk-table__header">School</th>
              <th className="govuk-table__header">School data</th>
              <th className="govuk-table__header">% of exp.</th>
            </tr>
            </thead>
            <tbody className="govuk-table__body">
            {schools && schools.map((school, i) => {
                const rating = school.categoryRatings.find(r => r.category === category);
                const schoolSpend = rating?.schoolData ?
                  `£${numberWithCommas(rating.schoolData.toFixed(2))}` : '-';
                const schoolPercent = rating?.percentageExpenditure ?
                  `${rating.percentageExpenditure.toFixed(2)}%` : '-';

                return (
                  <tr className="govuk-table__row" key={`school-${i}`}>
                    <td className="govuk-table__cell" data-label="School name">{school.name}</td>
                    <td className="govuk-table__cell" data-label="School data">{schoolSpend}</td>
                    <td className="govuk-table__cell" data-label="% of exp.">{schoolPercent}</td>
                  </tr>
                )
              }
            )}
            </tbody>
          </table>
        </div>
      </Modal>

      <Group position="right">
        <Button onClick={() => setOpened(true)}
                radius="xl"
                size="md"
                compact className="sfb-help-icon"
                title={`More about ${modalTitle}}`}>
          ?
        </Button>
      </Group>
    </>
  );
}