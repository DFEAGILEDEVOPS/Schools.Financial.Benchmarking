const { jsPDF } = window.jspdf;
class BenchmarkChartsViewModel {

    constructor() {

        sessionStorage.chartFormat = 'Charts';


        $(document).ready(() => {
            this.hideNaProgressScores();

            $("#benchmarkChartsList table.data-table-js.chart-table--mobile-only-view").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js.chart-table--mobile-only-view").first().find("thead th").length - 1, 1]]});
            $("#benchmarkChartsList table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]]});
            $("#benchmarkChartsList table.data-table-js.includes-table").tablesorter({ sortList: [[1, 1]] });
            $("#bestInClassTabSection table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#bestInClassTabSection table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });
            $("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-only-view").tablesorter({ sortList: [[$("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-only-view").first().find("thead th").length - 1, 1]] });
            $("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });

            $(".chart-table th").removeAttr("aria-label");
            
            DfE.Views.BenchmarkCharts = new BenchmarkChartManager();
            DfE.Views.BenchmarkCharts.generateCharts();

            this.refreshAddRemoveLinks();
            $('.save-as-image').show();

            $(document).keyup((e) => {
                if (e.key === "Escape") { // escape key maps to keycode `27`
                    $(".c3-tooltip-container").hide();
                }
            });

            $('.tabs li').height($('.tabs li').height() - 5);
        });

        GOVUK.Modal.Load();     
    }

    //This function is accessing to the scope of the AngularJS controller to retrieve the chart selections model and update the buttons accordingly in other tabs.
    refreshAddRemoveLinks() {

        function showRemoveLink(element) {
            $(element).find(".customRemove").show();
            $(element).find(".customAdd").hide();
        }

        function showAddLink(element) {
            $(element).find(".customAdd").show();
            $(element).find(".customRemove").hide();
        }

        let scope = angular.element($("#listCtrl")).scope();
        if (scope) {
            scope.dataLoaded.then(
                function () {
                    $(".customActions").each((i, el) => {
                        let chartId = Number($(el).attr("data-fn"));
                        let showValue = $(el).attr("data-sv");

                        _.forEach(scope.selectionList.HierarchicalCharts,
                            (group) => {
                                let selection = _.find(group.Charts,
                                    function (c) {
                                        return c.Id === chartId;
                                    });

                                if (selection) {
                                    switch (showValue) {
                                        case 'PerPupil':
                                            selection.PerPupilSelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'PerTeacher':
                                            selection.PerTeacherSelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'PercentageOfTotalExpenditure':
                                            selection.PercentageExpenditureSelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'PercentageOfTotalIncome':
                                            selection.PercentageIncomeSelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'AbsoluteMoney':
                                            selection.AbsoluteMoneySelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'AbsoluteCount':
                                            selection.AbsoluteCountSelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'HeadcountPerFTE':
                                            selection.HeadCountPerFTESelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'FTERatioToTotalFTE':
                                            selection.PercentageOfWorkforceSelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'NoOfPupilsPerMeasure':
                                            selection.NumberOfPupilsPerMeasureSelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                        case 'PercentageTeachers':
                                            selection.PercentageTeachersSelected ? showRemoveLink(el) : showAddLink(el);
                                            break;
                                    }
                                }
                            });
                    });
                });
        }
    }

    //This function is accessing to the scope of the AngularJS controller tab to retrieve and update its chart selections model.
    addRemoveYourCharts(chartId, showValue, checked, element) {
        let scope = angular.element($("#listCtrl")).scope();
        scope.$apply(() => {
            _.forEach(scope.selectionList.HierarchicalCharts,
                (group) => {
                    let selection = group.Charts.find((c) => {
                        return c.Id === chartId;
                    });

                    if (selection) {
                        switch (showValue) {
                            case 'PerPupil':
                                selection.PerPupilSelected = checked;
                                break;
                            case 'PerTeacher':
                                selection.PerTeacherSelected = checked;
                                break;
                            case 'PercentageOfTotalExpenditure':
                                selection.PercentageExpenditureSelected = checked;
                                break;
                            case 'PercentageOfTotalIncome':
                                selection.PercentageIncomeSelected = checked;
                                break;
                            case 'AbsoluteMoney':
                                selection.AbsoluteMoneySelected = checked;
                                break;
                            case 'AbsoluteCount':
                                selection.AbsoluteCountSelected = checked;
                                break;
                            case 'HeadcountPerFTE':
                                selection.HeadCountPerFTESelected = checked;
                                break;
                            case 'FTERatioToTotalFTE':
                                selection.PercentageOfWorkforceSelected = checked;
                                break;
                            case 'NoOfPupilsPerMeasure':
                                selection.NumberOfPupilsPerMeasureSelected = checked;
                                break;
                            case 'PercentageTeachers':
                                selection.PercentageTeachersSelected = checked;
                                break;
                        }
                    }
                });
            scope.ctrl.persist();
        });

        if (checked) {
            $(element).parents(".your-chart-controls").find(".view-your-charts").show();
            $(".custom").addClass("bold");
        } else {
            $(element).parents(".your-chart-controls").find(".view-your-charts").hide();
        }

        this.refreshAddRemoveLinks();
    }       

    selectGrouping(grouping, parentGrouping) {
        $("#ChartGroup").val(grouping);
        $("#ChartGroup").change();
        $("#BCHeader")[0].scrollIntoView();
        $("#ChartGroup").focus();
        $(".back-to-main-chart-group-button .js-parent-group").text(parentGrouping);
        $(".back-to-main-chart-group-button").show();
    }

    resetGrouping() {
        $('#ChartGroup').prop('selectedIndex', 0);
        $("#ChartGroup").change();
        $(".back-to-main-chart-group-button").hide();
    }

    rebuildCharts() {
        let tabParameter = $("#tabSelection").val();
        let chartGroupParameter = $("#ChartGroup").val();
        let unitParameter = $("select[name='ShowValue']").val();
        let centralFinancing = $("#CentralFinancing").val();
        let trustCentralFinancing = $("#TrustCentralFinancing").val();
        let formatParameter = sessionStorage.chartFormat;
        let type = $("#Type").val();
        let comparisonType = $("#ComparisonType").val();
        let bicComparisonOverallPhase = $("#BicComparisonOverallPhase").val();

        let url = "/benchmarkcharts/getcharts?revgroup=" +
            tabParameter +
            "&showValue=" +
            unitParameter +
            "&chartGroup=" +
            chartGroupParameter;

        if (centralFinancing) {
            url += "&centralFinancing=" + centralFinancing;
        }

        if (trustCentralFinancing) {
            url += "&trustCentralFinancing=" + trustCentralFinancing;
        }

        if (type) {
            url += "&type=" + type;
        }

        if (formatParameter) {
            url += "&format=" + formatParameter;
        }

        if (comparisonType) {
            url += "&comparisonType=" + comparisonType;
        }

        if (bicComparisonOverallPhase) {
            url += "&bicComparisonOverallPhase=" + bicComparisonOverallPhase;
        }

        $.ajax({
            url: url,
            datatype: 'json',
            beforeSend: () => {
                DfE.Util.LoadingMessage.display("#benchmarkChartsList", "Updating charts");
            },
            success: (data) => {
                $("#benchmarkChartsList").html(data);
                this.refreshAddRemoveLinks();
                $('.save-as-image').show();

                DfE.Views.BenchmarkCharts.generateCharts(unitParameter);

                window.GOVUKFrontend.initAll({ scope : $("#benchmarkChartsList")[0]});

                $("#benchmarkChartsList table.data-table-js.chart-table--mobile-only-view").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js.chart-table--mobile-only-view").first().find("thead th").length - 1, 1]] });
                $("#benchmarkChartsList table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });
                $("#benchmarkChartsList table.data-table-js.includes-table").tablesorter({ sortList: [[1, 1]] });
                $("#bestInClassTabSection table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#bestInClassTabSection table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });
                $("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-only-view").tablesorter({ sortList: [[$("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-only-view").first().find("thead th").length - 1, 1]] });
                $("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });

            }
        });
    }

    hideNaProgressScores() {
        if ($("#ComparisonSchoolsTable").length > 0) {
            let rowCount = $("#ComparisonSchoolsTable tbody tr").length;
            let naKs2Count = $("#ComparisonSchoolsTable tbody tr .td-ks2-js .score.na").length;
            let naP8Count = $("#ComparisonSchoolsTable tbody tr .td-p8-js .score.na").length;
            if (rowCount === naKs2Count) {
                let naKs2th = $("#ComparisonSchoolsTable thead tr th.th-ks2-js");
                let naKs2td = $("#ComparisonSchoolsTable tbody tr .td-ks2-js");
                naKs2th.hide();
                naKs2td.hide();
            }

            if (rowCount === naP8Count) {
                let naP8th = $("#ComparisonSchoolsTable thead tr .th-p8-js");
                let naP8td = $("#ComparisonSchoolsTable tbody tr .td-p8-js");
                naP8th.hide();
                naP8td.hide();
            }
        }
    }

    saveAsImage(name, id) {
        let svg = $('#' + id).find('svg')[0];
        d3.select(svg)
            .append('text')
            .attr('y', 17)
            .attr('text-anchor', 'start')
            .style('font-size', '24px')
            .style('font-weight', 'bold')
            .classed("title", true)
            .text($('#' + id).data("chart-title"));
        saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white'});

        d3.selectAll('#' + id + ' svg .title').remove(); 
    }

    downloadData(establishmentType) {
        $("#DownloadLinkTextWrapper").html("<span id='DownloadLinkText' role='alert' aria-live='assertive'> Downloading<span aria-hidden='true'>...</span></span>");
        document.getElementById('download_iframe').src = '/BenchmarkCharts/Download?type=' + establishmentType;
        setTimeout(() => {
            $("#DownloadLinkTextWrapper").html("<span id='DownloadLinkText'> Download data<span class='visually-hidden'> (CSV)</span></span>");
        }, 5000)
    }

    printPage() {

        let accordion_sections = $("#benchmarkChartsList .accordion-section");
        accordion_sections.attr('aria-expanded', true);

        let buttons = $("#benchmarkChartsList .chart-accordion-header");
        buttons.each(function () {
            $(this).attr('aria-label', $(this).attr('aria-label').replace("Show", "Hide"));
        });

        window.print();
    }

    changeTab(tab) {
        if (tab === "Custom") {
            $(".tabs li a span.bmtab").text("");
            $(".tabs li#" + tab + " a span.bmtab").text(" selected ");
            $("#tabsSection form").empty('');
            $("#tabsSection .sticky-chart-controls").empty('');
            $("#downloadLinkContainer").hide();
            $("#PrintLinkText").text(" Print report");
            $("#PdfLinkText").text(" Download report");
            $(".js-show-value-help").hide();
            let scope = angular.element($("#listCtrl")).scope();
            scope.ctrl.displayCustomReport();
            $('.sticky-div').Stickyfill();
        } else if (tab === "BestInClass") {
            $(".tabs li a span.bmtab").text("");
            $(".tabs li#" + tab + " a span.bmtab").text(" selected ");
            $("#downloadLinkContainer").show();
            $("#PrintLinkText").text(" Print page");
            $("#PdfLinkText").text(" Download page");
            $(".js-show-value-help").hide();
        }
        else if (tab === "ComparisonSchools") {
            $(".tabs li a span.bmtab").text("");
            $(".tabs li#" + tab + " a span.bmtab").text(" selected ");
            $("#downloadLinkContainer").show();
            $("#PrintLinkText").text(" Print page");
            $("#PdfLinkText").text(" Download page");
            $(".js-show-value-help").hide();
        }
        else {
            let unitParameter = $("select[name='ShowValue']").val();
            let financingParameter = $("#CentralFinancing").val();
            let trustFinancingParameter = $("#TrustCentralFinancing").val();
            unitParameter = unitParameter ? unitParameter : "AbsoluteMoney";
            let typeParameter = $("#Type").val();
            let comparisonType = $("#ComparisonType").val();
            let bicComparisonOverallPhase = $("#BicComparisonOverallPhase").val();
            let excludePartial = $("#ExcludePartial").val();
            let formatParameter = sessionStorage.chartFormat;
            let url = "/benchmarkcharts/tabchange?tab=" + tab +
                "&type=" + typeParameter +
                "&showValue=" + unitParameter;
            if (financingParameter) {
                url += "&financing=" + financingParameter;
            }
            if (trustFinancingParameter) {
                url += "&trustFinancing=" + trustFinancingParameter;
            }
            if (formatParameter) {
                url += "&format=" + formatParameter;
            }
            if (comparisonType) {
                url += "&comparisonType=" + comparisonType;
            }
            if (bicComparisonOverallPhase) {
                url += "&bicComparisonOverallPhase=" + bicComparisonOverallPhase;
            }
            if (excludePartial) {
                url += "&excludePartial=" + excludePartial;
            }
            $.ajax({
                url: url,
                datatype: 'json',
                beforeSend: () => {
                    DfE.Util.LoadingMessage.display(".sticky-chart-controls", "Updating charts");
                    $("#tabsSection form").hide();
                },
                success: (data) => {
                    $(".tabs li a span.bmtab").text("");
                    $("#downloadLinkContainer").show();
                    $("#PrintLinkText").text(" Print page");
                    $("#PdfLinkText").text(" Download page");
                    let stickyDivHtml = $(data).find(".sticky-div")[0];
                    $("#tabsSection .sticky-chart-controls").replaceWith(stickyDivHtml);
                    let formHtml = $(data).find("form").html();
                    $("#tabsSection form").html(formHtml);
                    $("#tabsSection form").show();
                    $('.sticky-div').Stickyfill();  
                    $("#benchmarkChartsList table.data-table-js.chart-table--mobile-only-view").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js.chart-table--mobile-only-view").first().find("thead th").length - 1, 1]] });
                    $("#benchmarkChartsList table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });
                    $("#benchmarkChartsList table.data-table-js.includes-table").tablesorter({ sortList: [[1, 1]] });
                    $("#bestInClassTabSection table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#bestInClassTabSection table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });
                    $("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-only-view").tablesorter({ sortList: [[$("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-only-view").first().find("thead th").length - 1, 1]] });
                    $("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#comparisonSchoolsTabSection table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });

                    let unitParameter = $("select[name='ShowValue']").val();
                    this.refreshAddRemoveLinks();
                    $('.save-as-image').show();
                    if (tab === "Workforce") {
                        $(".js-show-value-help").show();
                    }
 
                    DfE.Views.BenchmarkCharts.generateCharts(unitParameter);
                }
            });
        }
    }

    toggleChartsTables(mode) {
        let $charts = $('.chart-wrapper');
        let $chartsScores = $('.chart-scores-wrapper');
        let $tables = $('.chart-table-wrapper');
        let $showChartsButton = $('.view-charts-tables.charts');
        let $showTablesButton = $('.view-charts-tables.tables');
        let $saveAsImagesButtons = $('.save-as-image');
        let $viewMoreLabels = $("a span.view-more");
        let $comparisonType = $("#ComparisonType");
        if (mode === 'Charts') {
            $showChartsButton.hide();
            $showTablesButton.show();
            $tables.hide();
            $charts.css('display', 'inline-block');
            $chartsScores.css('display', 'flex');
            $saveAsImagesButtons.show();
            $viewMoreLabels.text("View more charts");
            sessionStorage.chartFormat = 'Charts';
            if ($comparisonType.val() === "BestInClass") {
                this.rebuildCharts();
            }
        } else if (mode === 'Tables') {
            $showTablesButton.hide();
            $showChartsButton.show();
            $charts.hide();
            $chartsScores.hide();
            $tables.show();
            $saveAsImagesButtons.hide();
            $viewMoreLabels.text("View more tables");
            sessionStorage.chartFormat = 'Tables';
        }
    }

    hideShowDetails(element) {
        let $tableWrapper = $(element).closest('.chart-table-wrapper');
        $tableWrapper.find('.detail').toggle(200);
        setTimeout(() => $tableWrapper.find('.hide-show-button:visible').focus(), 300);        
    }

    hideShowDetailsMobile(element) {
        let $tableWrapper = $(element).closest('.chart-table-wrapper');
        $tableWrapper.find('.detail').toggle(200);
        $tableWrapper.find('.chart-table--mobile-only-view').toggle(200);
        setTimeout(() => $tableWrapper.find('.mobile-hide-show-button:visible').focus(), 300);    
    }

    downloadPage() {
        let downloadFormat = $("input:radio[name='downloadFormat']:checked").val();
        if (downloadFormat === "pdf") {
            this.pdfPage();
        } else {
            this.pptPage();
        }
    }

    pptPage() {
        $('#criteria-details.criteria-details').attr('open', 'true');
        $('.chart-container .accordion-section[aria-expanded="false"] .accordion-section-header').click();
        
        let pptGenerator = new PptGenerator();

        pptGenerator.writeHeadings();

        pptGenerator.writeWarnings();

        pptGenerator.writeTabs();

        pptGenerator.writeLastYearMessage();

        pptGenerator.writeCharts().then(() => {
            pptGenerator.writeCriteria();
            pptGenerator.writeComparisonSchools().then(() => {
                pptGenerator.writeBicSchools().then(() => {
                    pptGenerator.writeContextData().then(() => {
                        pptGenerator.save();
                    });
                });
            }); 
        });       
    }

    pdfPage() {

        $('#criteria-details.criteria-details').attr('open', 'true');

        let pdfGenerator = new PdfGenerator();

        pdfGenerator.writeHeadings();

        pdfGenerator.writeWarnings();

        pdfGenerator.writeTabs();

        pdfGenerator.writeLastYearMessage();

        pdfGenerator.writeComparisonSchools().then(() => {
            pdfGenerator.writeBicSchools().then(() => {
                pdfGenerator.writeCharts().then(() => {
                    pdfGenerator.writeCriteria().then(() => {
                        pdfGenerator.writeContextData().then(() => {
                            pdfGenerator.save();
                        });
                    });
                });
            });
        });
    }

    submitCriteriaForm() {
        $('form#advancedCriteria').submit();
    }

    saveBenchmarkBasketModal() {
        let link;
        let type = $('#Type').val();
        if (type === 'MAT') {
            let listCookie = GOVUK.cookie("sfb_comparison_list_mat");
            if (listCookie) {
                let matList = JSON.parse(listCookie);
                let cnoList = Array.from(matList.T, t => t.CN);
                let defaultCompanyNo = matList.DTCN;
                link = `${window.location.origin}/BenchmarkCharts/GenerateFromSavedBasket?companyNumbers=${cnoList.join('-')}`;
                if (defaultCompanyNo) {
                    link += `&default=${defaultCompanyNo}`;
                }
            }
        } else {
            let listCookie = GOVUK.cookie("sfb_comparison_list");
            if (listCookie) {
                let schoolList = JSON.parse(listCookie);
                let urnList = Array.from(schoolList.BS, s => s.U);
                let defaultUrn = schoolList.HSU;
                link = `${window.location.origin}/BenchmarkCharts/GenerateFromSavedBasket?urns=${urnList.join('-')}`;
                if (defaultUrn) {
                    link += `&default=${defaultUrn}`;
                }
            }
            let comparison = $('#ComparisonType').val();
            if (comparison === "BestInClass") {
                link += "&comparison=BestInClass";
            }
        }

        let $body = $('body');
        let $page = $('#js-modal-page');
        var $modal_code = `<dialog id='js-modal' class='modal govuk-body-s' role='dialog' aria-labelledby='modal-title'>
        <div role='document' class='save-modal-js page-1' style='display: block'>
            <a href='#' id='js-modal-close' class='modal-close' data-focus-back='SaveLink' title='Close'>Close</a>
            <h1 id='modal-title' class='modal-title'>Save or share benchmark</h1>
            <p id='modal-content' class='govuk-!-font-size-19'>
                Save your benchmark by copying the link below and saving it as a bookmark or in a document. Alternatively you can email the link to yourself or share with others.
            </p>
            <div class='govuk-form-group govuk-!-margin-bottom-0'><label class='govuk-label' for='saveUrl'>Page link</label>
                <input id='saveUrl' name='saveUrl' type='text' class='govuk-input save-url-input' value='${link}'>
                <button id='clip-button' class='govuk-button' data-module='govuk-button' type='button' data-clipboard-target='#saveUrl' >Copy link to clipboard</button>
                <span id='clip-not-supported' class='govuk-error-message' style='display: none'>Please select and copy the link above.</span>
            </div>         
            <a class='govuk-link email-the-link' href="mailto:?subject=Saved%20benchmark%20charts&body=Here%20is%20your%20saved%20benchmark%20basket:%20${link}">
            <img class="icon email-list-icon" src="/public/assets/images/icons/icon-email.png" alt="" />Email the link</a>            
        </div>
        <div role='document' class='save-modal-js page-2' style='display: none'>
            <a href='#' id='js-modal-close' class='modal-close' data-focus-back='SaveLink' title='Close'>Close</a>
            <h1 id='modal-title' class='modal-title'>Link copied to clipboard</h1>
            <p id='modal-content'>
                You can now save the link as a bookmark or in a document to keep your benchmark basket.
            </p>           
            <a class='govuk-link' href='javascript: DfE.Views.BenchmarkChartsViewModel.showSaveModalOne()'>See more options to save</button>            
        </div>
        <a href='#' id='js-modal-close-bottom' class='modal-close white-font' data-focus-back='SaveLink' title='Close'>Close</a>
        </dialog>`;

        $($modal_code).insertAfter($page);
        $body.addClass('no-scroll');
        $page.attr('aria-hidden', 'true');

        // add overlay
        var $modal_overlay =
            '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

        $($modal_overlay).insertAfter($('#js-modal'));

        let clipboard = new ClipboardJS('#clip-button');

        clipboard.on('success', function () {
            DfE.Views.BenchmarkChartsViewModel.showSaveModalTwo();
        });

        if (!ClipboardJS.isSupported()) {
            $('#clip-button').hide();
            $('#clip-not-supported').show();
        }


        $('#js-modal-close').focus();
    }

    renderMissingFinanceInfoModal(isMAT) {
        var $body = $('body');
        var $page = $('#js-modal-page');

        var $modal_code = "<dialog id='js-modal' class='modal govuk-body-s' role='dialog' aria-labelledby='modal-title'><div role='document'>" +
            "<a href='#' id='js-modal-close' class='modal-close' title='Close'>Close</a>" +
            "<h1 id='modal-title' class='modal-title'>Incomplete financial data</h1>";
        if (isMAT) {
            $modal_code += "<p id='modal-content'>Some of this trust's schools have data from a period less than 12 months.</p>";
        } else {
            $modal_code += "<p id='modal-content'>This school doesn't have a complete set of financial data for this period.</p>";
        }

        $modal_code += "</div><a href='#' id='js-modal-close-bottom' class='modal-close white-font' title='Close'>Close</a></dialog>";

        $($modal_code).insertAfter($page);
        $body.addClass('no-scroll');

        $page.attr('aria-hidden', 'true');

        // add overlay
        var $modal_overlay =
            '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

        $($modal_overlay).insertAfter($('#js-modal'));

        $('#js-modal-close').focus();
    }
        
    showSaveModalOne() {
        $('.save-modal-js.page-2').hide();
        $('.save-modal-js.page-1').show();
    }

    showSaveModalTwo() {
        $('.save-modal-js.page-1').hide();
        $('.save-modal-js.page-2').show();
    }

    renderDownloadModal(event) {

        event.stopPropagation();

        var $body = $('body');
        var $page = $('#js-modal-page');

        var $modal_code = "<dialog id='js-modal' class='modal govuk-body-s' role='dialog' aria-labelledby='modal-title'><div role='document'>" +
            "<a href='#' id='js-modal-close' class='modal-close' data-focus-back='PdfLink' title='Close'>Close</a>" +
            "<h1 id='modal-title' class='modal-title'>Select file format</h1>" +            
            `<div class="govuk-form-group">
              <fieldset class="govuk-fieldset">

                <legend class="govuk-fieldset__legend">
                  You can download the page's charts in PDF or PowerPoint format.
                </legend>
                <div class="govuk-radios">
                    <div class="govuk-radios__item">
                      <input class="govuk-radios__input" id="radio-1" type="radio" name="downloadFormat" value="pdf" checked>
                      <label for="radio-1" class="govuk-label govuk-radios__label">PDF format</label>
                    </div>
                    <div class="govuk-radios__item">
                      <input class="govuk-radios__input" id="radio-2" type="radio" name="downloadFormat" value="ppt">
                      <label for="radio-2" class="govuk-label govuk-radios__label">PowerPoint format</label>
                    </div>
                </div>
              </fieldset>
            <div class="grid-row modal-form-buttons">
                <div class="column-half">
                    <div class="govuk-button-group">
                        <button type="button" data-module="govuk-button" class="govuk-button" onclick="DfE.Views.BenchmarkChartsViewModel.downloadPage(); GOVUK.Modal.prototype.closeAccessibleModal(event);">Download</button>
                        <a class="govuk-link" href="javascript: GOVUK.Modal.prototype.closeAccessibleModal(event);">Cancel</button>
                    </div>
                </div>
            </div>
            </div>` +
            "</div><a href='#' id='js-modal-close-bottom' class='modal-close white-font' data-focus-back='PdfLinkText' title='Close'>Close</a></dialog>";

        $($modal_code).insertAfter($page);
        $body.addClass('no-scroll');

        $page.attr('aria-hidden', 'true');

        // add overlay
        var $modal_overlay =
            '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

        $($modal_overlay).insertAfter($('#js-modal'));

        $('#js-modal-close').focus();

    }
}

class PptGenerator {

    constructor() {
        this.doc = new PptxGenJS();
        this.slide = this.doc.addNewSlide();
        this.yOffset = 0;
    }

    writeHeadings() {
        this.yOffset += 0.5;
        this.slide.addText('Schools Financial Benchmarking', { x: 0.2, y: this.yOffset, fontSize: 18, bold: true  });

        this.yOffset += 0.5;
        this.slide.addText($('#BCHeader').get(0).innerText, { x: 0.2,y: this.yOffset, fontSize: 16, bold: true });

        if ($('#comparing-text').length > 0) {
            this.yOffset += 0.5;
            this.slide.addText($('#comparing-text').get(0).innerText, { x: 0.2,y: this.yOffset, fontSize: 12, bold: true, w: '90%' });
        }
    }

    writeWarnings() {
        let warnings = $('.govuk-inset-text--orange .combined-warnings');
        if (warnings.length > 0) {
            warnings.each((index, element) => {
                this.yOffset += 0.5;
                this.slide.addText(element.innerText, { x: 0.2, y: this.yOffset, fontSize: 12, italic: true, color: 'f47738', w: '90%' });
            });
        }
    }

    writeTabs() {
        this.yOffset += 0.2;
        if ($('.tabs li.active').length > 0) {
            this.yOffset += 0.5;
            if ($('.tabs li.active').get(0).innerText.indexOf('Your') < 0) {                
                this.slide.addText($('.tabs li.active').get(0).innerText.replace('selected', ''), { x: 0.2, y: this.yOffset, fontSize: 12, bold: true });
            } else {
                this.slide.addText('Your charts', { x: 0.2, y: this.yOffset, fontSize: 12, bold: true });
            }
        }

        let filters = $('.chart-filter:visible');
        if (filters.length > 0) {
            filters.each((index, element) => {                
                this.yOffset += 0.3;
                this.slide.addText($(element).find('label').get(0).innerText + ': ' + $(element).find('option[selected]').get(0).innerText, { x: 0.2, y: this.yOffset, fontSize: 12 });
            });
        }
    }

    writeLastYearMessage() {
        this.yOffset += 0.5;
        this.slide.addText('', { x: 0.2, y: this.yOffset, fontSize: 12, line: { pt: '2', color: 'A9A9A9' }, w: '95%' });
        if ($('.latest-year-message:visible').length > 0) {
            this.yOffset += 0.2;
            this.slide.addText($('.latest-year-message:visible').get(0).innerText, { x: 0.2, y: this.yOffset, fontSize: 12, w: '95%' });
        }
    }

    writeCharts() {
        return new Promise((resolve) => {
            let charts = $('.chart-container:visible');
            let yValuesCount = JSON.parse($(".chart").first().attr('data-chart')).length;
            let chartPerPage = Math.ceil(5 / yValuesCount);

            let chartImageResults = [];
            (async () => {
                for (var index = 0; index < charts.length; index++) {
                    if (index % chartPerPage === 0) {
                        this.slide = this.doc.addNewSlide();
                        this.yOffset = 0;
                    } else {
                        this.yOffset += (4 / chartPerPage);
                    }
                    let header = $(charts[index]).find('h2').get(0).innerText;
                    if (header.length < 60) {
                        this.yOffset += 0.2;
                        this.slide.addText(header, { x: 0.4, y: this.yOffset, fontSize: 16, bold: true });
                    } else {
                        let header1 = header.substring(0, header.lastIndexOf('('));
                        let header2 = header.substring(header.lastIndexOf('('));
                        this.yOffset += 0.2;
                        this.slide.addText(header1, { x: 0.4, y: this.yOffset, fontSize: 16, bold: true });
                        this.yOffset += 0.2;
                        this.slide.addText(header2, { x: 0.4, y: this.yOffset, fontSize: 16, bold: true });
                    }
                    if (sessionStorage.chartFormat === 'Charts') {
                        chartImageResults.push(await this.pptWriteChart(`#chart_${index}`, chartPerPage));
                    } else {
                        this.pptWriteTable(`#table_for_chart_${index}`, chartPerPage);
                    }
                }
            })();
            
            if (sessionStorage.chartFormat === 'Charts') {
                var intervalId = setInterval(() => {
                    if (chartImageResults.length === charts.length) {
                        clearInterval(intervalId);
                        resolve();
                    }
                }, 100);
            } else {
                resolve();
            } 
        });
    }

    writeCriteria() {
            if ($('#criteriaTable').length > 0 && $('#criteriaTable').is(":visible")) {
                this.slide = this.doc.addNewSlide();
                this.yOffset = 0.2;
                if ($('#criteriaExp').length > 0) {
                    this.slide.addText($('#criteriaExp').get(0).innerText, { x: 0.4, y: this.yOffset, fontSize: 12, w: '95%' });
                }
                this.pptWriteTable('#criteriaTable', 1);                
            }
    }

    writeBicSchools() {
        return new Promise((resolve, reject) => {
            if ($('#ProgressScoresTable:visible').length > 0) {
                this.slide = this.doc.addNewSlide();
                this.yOffset = 0.1;
                this.slide.addText($('#ProgressScoresTableHeading').get(0).innerText, { x: 0.4, y: this.yOffset, fontSize: 10 });

                this.pptGenerateImage('#ProgressScoresTable').then((canvas) => {
                    let img = canvas.toDataURL("image/png");
                    let ratio = canvas.width / canvas.height;
                    this.yOffset += 0.3;
                    this.slide.addImage({ data: img, x: 0.5, y: this.yOffset, w: 4.0, h: 4 / ratio } );
                    resolve();
                });
            }else {
                resolve();
            }
        });
    }

    writeComparisonSchools() {
        return new Promise((resolve, reject) => {
            if ($('#ComparisonSchoolsTable:visible').length > 0) {
                this.slide = this.doc.addNewSlide();
                this.yOffset = 0.1;

                this.pptGenerateImage('#ComparisonSchoolsTable').then((canvas) => {
                    let img = canvas.toDataURL("image/png");
                    let ratio = canvas.width / canvas.height;
                    this.yOffset += 0.3;
                    let width = 4.5;
                    let height = 4.5 / ratio;
                    if (height > 5) {
                        height = 5;
                        width = height * ratio;
                    }
                    this.slide.addImage({ data: img, x: 0.5, y: this.yOffset, w: width, h: height });
                    resolve();
                });
            } else {
                resolve();
            }
        });
    }

    writeContextData() {
        return new Promise((resolve, reject) => {
            if ($('#contextDataTable:visible').length > 0) {
                this.slide = this.doc.addNewSlide();
                this.yOffset = 0.2;
                this.slide.addText($('#contextExp').get(0).innerText, { x: 0.4, y: this.yOffset, fontSize: 16, bold: true });

                this.pptWriteTable('#contextDataTable', 1);
            }
            resolve();
        });
    }

    save() {
        this.doc.save('sfb-benchmark-charts');
    }

    pptWriteChart(chartId, chartPerPage) {
        return new Promise((resolve) => {
            let img = $(chartId).data('img');
            let svg = $(chartId).find('svg')[0];
            if (img) {
                console.log("Using already generated image for chart exporting");
                this.pptWriteChartWithRatio(svg.clientWidth, svg.clientHeight, chartPerPage, img);
                resolve('done');
            }
            else {
                saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white' },
                    (img) => {
                        console.log("Generating new image for chart exporting");
                        this.pptWriteChartWithRatio(svg.clientWidth, svg.clientHeight, chartPerPage, img);
                        resolve('done');
                    });
            }
        });
    }

    pptWriteChartWithRatio(clientWidth, clientHeight, chartPerPage, img) {
        this.yOffset += 0.5;
        let ratio = clientWidth / clientHeight;
        let height = 4 / chartPerPage;
        let width = height * ratio;
        if (width > 8) { //not to overflow horizontally
            width = 8;
            height = width / ratio;
        }
        this.slide.addImage({ data: img, x: 0.5, y: this.yOffset, w: width, h: height });
    }

    pptWriteTable(tableId, tablePerPage) {
        let rows = [];
        let headers = [];
        $(`${tableId} th:visible`).toArray().map((th) => {
            headers.push({ text: th.attributes['data-header'] === undefined ? th.textContent : th.attributes['data-header'].value, options: { bold: true } });
        });
        let data = $(`${tableId} tbody tr:visible`).toArray().map((tr) => {
            let trArr = [];
            $(tr).children('td').toArray().map((td) => {
                trArr.push(td.textContent.trim());
            });
            return trArr;
        });
        rows.splice(0, 0, headers);
        rows = rows.concat(data);
        this.yOffset += 0.5;
        this.slide.addTable(rows, { x: 0.5, y: this.yOffset, fontSize: 10, w: 9.0, color: '363636', autoPage: tablePerPage === 1 });
    }

    pptGenerateImage(element) {

        function getCanvas(element) {
            return html2canvas($(element)[0], {
                imageTimeout: 2000,
                removeContainer: true
            });
        }

        return getCanvas(element);
    }
}

class PdfGenerator {

    constructor() {
        this.MARGIN_LEFT = 20;
        this.doc = new jsPDF({ unit: 'px', format: 'a3' });
        this.offset = 60;
    }

    pdfGenerateImage(element) {

        function getCanvas(element) {
            return html2canvas($(element)[0], {
                imageTimeout: 2000,
                removeContainer: true
            });
        }

        return getCanvas(element);
    }

    pdfAddImage(canvas, width, height) {
        let img = canvas.toDataURL("image/png");
        if (width && height) {
            this.doc.addImage(img, 'JPEG', this.MARGIN_LEFT, this.offset, width, height);
        } else {
            this.doc.addImage(img, 'JPEG', this.MARGIN_LEFT, this.offset);
        }
    }

    pdfWriteLine(type, text) {
        this.doc.setFont("helvetica");
        this.doc.setTextColor(0, 0, 0);
        let fontSize;
        switch (type) {
            case 'H1':
                this.doc.setFont(undefined,"bold");
                fontSize = 40;
                break;
            case 'H2':
                this.doc.setFont(undefined, "bold");
                fontSize = 30;
                break;
            case 'H3':
                this.doc.setFont(undefined, "bold");
                fontSize = 20;
                break;
            case 'Warning':
                this.doc.setFont(undefined, "italic");
                this.doc.setTextColor(244, 119, 56);
                fontSize = 20;
                break;
            case 'Info':
                this.doc.setFont(undefined, "italic");
                fontSize = 15;
                break;
            default:
                this.doc.setFont(undefined, "normal");
                fontSize = 15;
        }

        this.doc.setFontSize(fontSize);
        this.doc.text(this.MARGIN_LEFT, this.offset, text);
        this.offset += fontSize + 8;
    }

    pdfAddHorizontalLine() {
        this.doc.line(this.MARGIN_LEFT, this.offset, 620, this.offset);
        this.offset += 18;
    }

    pdfAddNewPage() {
        this.doc.addPage('a3', 'portrait');
        this.offset = 70;
    }

    pdfWriteChart(index, chartPerPage, element) {
        return new Promise((resolve) => {
            let img = $('#chart_' + index).data('img');
            let svg = $('#chart_' + index).find('svg')[0];
            if (img) {
                console.log("Using already generated image for chart exporting");
                this.pdfWriteChartWithHeaders(index, chartPerPage, element, img);
                resolve('done');
            }
            else {
                saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white' },
                    (img) => {
                        console.log("Generating new image for chart exporting");
                        this.pdfWriteChartWithHeaders(index, chartPerPage, element, img);
                        resolve('done');
                    });
            }
        });
    }

    pdfWriteChartWithHeaders(index, chartPerPage, element, img) {
        if (index % chartPerPage === 0) {
            this.pdfAddNewPage();
        } else {
            this.offset += (800 / chartPerPage);
        }
        let header = $(element).find('h2').get(0).innerText;
        if (header.length < 60) {
            this.pdfWriteLine('H3', header);
        } else {
            let header1 = header.substring(0, header.lastIndexOf('('));
            let header2 = header.substring(header.lastIndexOf('('));
            this.pdfWriteLine('H3', header1);
            this.pdfWriteLine('H3', header2);
        }
        this.doc.addImage(img, 'JPEG', 0, this.offset);
    }

    pdfWriteTable(id, index, chartPerPage, element) {
        if (index % chartPerPage === 0) {
            this.pdfAddNewPage();
        } else {
            this.offset += (800 / chartPerPage);
        }
        let header = $(element).find('h2').get(0).innerText;
        if (header.length < 60) {
            this.pdfWriteLine('H3', header);
        } else {
            let header1 = header.substring(0, header.lastIndexOf('('));
            let header2 = header.substring(header.lastIndexOf('('));
            this.pdfWriteLine('H3', header1);
            this.pdfWriteLine('H3', header2);
        }

        let headers = $(id + ' th:visible').toArray().map((th) => {
             return th.attributes['data-header'].value;
        });
        let data = $(id + ' tbody tr:visible').toArray().map((tr) => {
            let trObj = {};
            $(tr).children('td').toArray().map((td) => {
                if (td.children.length > 0) {
                    trObj[td.attributes['data-header'].value] = td.children[0].textContent.trim();
                } else {
                    trObj[td.attributes['data-header'].value] = td.textContent.trim();
                }
            });
            return trObj;
        });

        this.doc.table(this.MARGIN_LEFT, this.offset, data, headers, { autoSize: true, fontSize: 9, margins: { bottom: 5, left: 5, top: 5, width: 600 } });
    }

    pdfSave() {
        this.doc.save('sfb-benchmark-charts.pdf');
    }

    writeHeadings() {

        this.pdfWriteLine('H1', 'Schools Financial Benchmarking');

        this.pdfWriteLine('H2', $('#BCHeader').get(0).innerText);

        if ($('#comparing-text').length > 0) {
            this.pdfWriteLine('H4', $('#comparing-text').get(0).innerText);
        }
    }

    writeWarnings() {

        let warnings = $('.govuk-inset-text--orange .combined-warnings');
        if (warnings.length > 0) {
            warnings.each((index, element) => {
                this.pdfWriteLine('Warning', element.innerText);
            });
        }
    }

    writeTabs() {

        this.offset += 10;

        if ($('.tabs li.active').length > 0) {
            if ($('.tabs li.active').get(0).innerText.indexOf('Your') < 0) {
                this.pdfWriteLine('H3', $('.tabs li.active').get(0).innerText.replace('selected', ''));
            } else {
                this.pdfWriteLine('H3', 'Your charts');
            }
        }

        let filters = $('.chart-filter:visible');
        if (filters.length > 0) {
            filters.each((index, element) => {
                this.pdfWriteLine('Normal', $(element).find('label').get(0).innerText + ': ' + $(element).find('option[selected]').get(0).innerText);
            });
        }
    }

    writeLastYearMessage() {
        this.pdfAddHorizontalLine();
        if ($('.latest-year-message:visible').length > 0) {
            this.pdfWriteLine('Info', $('.latest-year-message:visible').get(0).innerText);
        }

    }

    writeComparisonSchools() {
        return new Promise((resolve, reject) => {
            if ($('#ComparisonSchoolsTable:visible').length > 0) {   
                this.pdfGenerateImage('#ComparisonSchoolsTable').then((canvas) => {
                    if (canvas.height > 1060) {
                        this.pdfAddNewPage();
                        this.offset = 0;
                        let ratio = canvas.width / canvas.height;
                        let height = 880;
                        let width = 880 * ratio;
                        if (width > 550) {
                            width = 550;
                            height = width / ratio;
                        }
                        this.pdfAddImage(canvas, width, height);
                    } else {
                        this.pdfAddImage(canvas);
                    }
                    resolve();
                });
            } else {
                resolve();
            }
        });
    }

    writeBicSchools() {
        return new Promise((resolve, reject) => {
            if ($('#ProgressScoresTable').length > 0 && $('#ProgressScoresTable').is(":visible")) {
                //this.pdfAddNewPage();
                this.pdfWriteLine('H4', $('#ProgressScoresTableHeading').get(0).innerText);                
                //this.pdfWriteLine('Normal', $('.show-count-js').get(0).innerText);                
                this.pdfGenerateImage('#ProgressScoresTable').then((canvas) => {
                    this.pdfAddImage(canvas);
                    resolve();
                });
            } else {
                resolve();
            }
        });
    }

    writeCharts() {        
        return new Promise((resolve) => {
            let charts = $('.chart-container:visible');
            let yValuesCount = JSON.parse($(".chart").first().attr('data-chart')).length;
            let chartPerPage = Math.ceil(12 / yValuesCount);

            let chartImageResults = [];
            (async () => {
                for (var i = 0; i < charts.length; i++) {
                    if (sessionStorage.chartFormat === 'Charts') {
                        chartImageResults.push(await this.pdfWriteChart(i, chartPerPage, charts[i]));
                    } else {
                        this.pdfWriteTable('#table_for_chart_' + i, i, chartPerPage, charts[i]);
                    }
                }
            })();

            if (sessionStorage.chartFormat === 'Charts') {
                let intervalId = setInterval(() => {
                    if (chartImageResults.length === charts.length) {
                        clearInterval(intervalId);
                        resolve();
                    }
                }, 100);
            } else {
                resolve();
            }         
        });
    }

    writeCriteria() {
        return new Promise((resolve, reject) => {
            if ($('#criteriaTable').length > 0 && $('#criteriaTable').is(":visible")) {
                this.pdfAddNewPage();
                if ($('#criteriaExp').length > 0) {
                    this.pdfWriteLine('Normal', $('#criteriaExp').get(0).innerText);
                }
                this.pdfGenerateImage('#criteriaTable').then((canvas) => {
                    this.pdfAddImage(canvas);
                    resolve();
                });
            } else {
                resolve();
            }
        });
    }

    writeContextData() {
        return new Promise((resolve, reject) => {
            if ($('#contextDataTable').length > 0 && $('#contextDataTable').is(":visible")) {
                this.pdfAddNewPage();
                this.pdfWriteLine('H2', $('#contextExp').get(0).innerText);
                this.pdfGenerateImage('#contextDataTable').then((canvas) => {
                    this.pdfAddImage(canvas);
                    resolve();
                });
            } else {
                resolve();
            }
        });
    }

    save() {
        this.pdfSave();
    }
}