(function (GOVUK, DfE) {
    'use strict';

    $(document).ready(function () {
        // Block Labels
        var toggleBlockLabel = new DfE.Elements.BlockLabel();
        toggleBlockLabel.showHideRadioToggledContent();
        toggleBlockLabel.showHideCheckboxToggledContent();

        // Selection Buttons
        var $buttons = $(".block-label input[type='radio'], .block-label input[type='checkbox']");
        new GOVUK.SelectionButtons($buttons);
    });

}(GOVUK, DfE));