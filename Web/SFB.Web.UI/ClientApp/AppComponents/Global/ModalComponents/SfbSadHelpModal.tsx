import React, {useState} from 'react';
import {Modal, Button, Grid} from '@mantine/core';
import {SadThreshold} from '../../SadTrustApp/Models/sadTrustTablesModels';
import {numberWithCommas} from '../../Helpers/formatHelpers';

interface Props {
  modalTitle?: string;
  modalContent?: string;
  thresholds?: SadThreshold[];
  establishmentThreshold?: SadThreshold;
  establishmentName?: string;
  columnHeading?: string;
  unitFormat?: string;
  useExclaimIcon?: boolean;
}

export default function SfbSadHelpModal(props: Props) {
  const [opened, setOpened] = useState(false);

  return (
    <>
      <Modal opened={opened}
             onClose={() => setOpened(false)}
             withCloseButton={false}
             size="xl">
        <div className="modal-header">
          <div className="sfb-modal__header">
            <h2 className="govuk-heading-m">{props.modalTitle}</h2>
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
          <div className={props.establishmentThreshold ? "sfb-modal--column" : ""}>
            {props.modalContent &&
              <div dangerouslySetInnerHTML={{__html: props.modalContent}}/>}
          </div>

          {props.establishmentThreshold &&
            <div className="sfb-modal--column">
              <h3 className="govuk-heading-s">{props.establishmentName}</h3>
              <table className="govuk-table">
                <thead className="govuk-table__head">
                <tr className="govuk-table__row">
                  <th className="govuk-table__header">{props.columnHeading}</th>
                  <th className="govuk-table__header">Rating against thresholds</th>
                </tr>
                </thead>
                <tbody className="govuk-table__body">
                {
                  props.thresholds?.map((threshold, idx) => {
                    const panelClassName = threshold.RatingText === props.establishmentThreshold?.RatingText ? 'sfb-highlight' : '';
                    return (
                      <tr className="govuk-table__row"
                          key={threshold.RatingText.replaceAll(' ', '') + idx.toString()}>
                        <td className="govuk-table__cell">

                          <div className={panelClassName}>
                            {(props.unitFormat === 'percentage') &&
                              <>{(threshold.ScoreLow * 100).toFixed(1)}%
                                {threshold.ScoreHigh ? 
                                `- ${(threshold.ScoreHigh * 100).toFixed(1)}%` :
                                ` and above`
                                }
                              </>
                            }
                            {(props.unitFormat === 'currency' && threshold.ScoreHigh) &&
                              <>£{numberWithCommas(threshold.ScoreLow.toFixed(2))} -
                                £{numberWithCommas(threshold.ScoreHigh.toFixed(2))}</>
                            }
                            {(props.unitFormat === 'currency' && !threshold.ScoreHigh) &&
                              <>£{numberWithCommas(threshold.ScoreLow.toFixed(2))} and above</>
                            }
                            {(typeof props.unitFormat === 'undefined') &&
                              <>{threshold.ScoreLow} - {threshold.ScoreHigh}</>
                            }
                          </div>
                        </td>
                        <td className="govuk-table__cell">
                          <div className={`rating-box ${threshold.RatingColour}`}>
                            {threshold.RatingText}
                          </div>
                        </td>
                      </tr>
                    );
                  })
                }
                </tbody>
              </table>
            </div>
          }
        </div>

      </Modal>
      <Button onClick={() => setOpened(true)}
              radius="xl"
              size="md"
              compact className="sfb-help-icon"
              title={`More about ${props.modalTitle} at ${props.establishmentName}`}>
        {props.useExclaimIcon ? "!" : "?"}
      </Button>
    </>
  )
}