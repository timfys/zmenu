const fileTypes = [
    'image/jpeg',
    'image/pjpeg',
    'image/png',
    'image/jpg'
]
const MaximumFileSize = 1048576;
let paymenttoEditId = -1;
let currentPage = 0;
const PageSize = 17;
let stackRequest = 0;
//let imageCahanged = false;
//const img = document.getElementById('UploadedImg');
//const observerConfig = { attributes: true, childList: true, subtree: true };
//const callback = onImageChange;
//const imageObserver = new MutationObserver(callback);
//imageObserver.observe(img, observerConfig);
$(document).ready(function ()
{
    ModelOnView();
    //onImageChange();
    checkButton();
    $(".bd-AddEdit-modal").on("hidden.bs.modal", function () {
        clearModal();
    });
    $("#paymentType").change(function ()
    {
        checkCommisionFields();
    });
    let input = document.getElementById("txtSearchPayment");
    input.addEventListener("keyup", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            document.getElementById("btnSearchPayment").click();
        }
    }); 
    //for two modal 
    $(document).on('show.bs.modal', '.modal', function (event) {
        var zIndex = 1040 + (10 * $('.modal:visible').length);
        $(this).css('z-index', zIndex);
        setTimeout(function () {
            $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
        }, 0);
    });
});
/*
function onImageChange()
{
    if ($('#UploadedImg').attr('src') == "" || $('#UploadedImg').attr('src') == null ||
    $('#UploadedImg').attr('src') == undefined || $('#UploadedImg').attr('src') == "//:0")
    {
        document.getElementById('RemoveImgBtn').style.display = "none";
    }
    else
    {
        document.getElementById('RemoveImgBtn').style.display = "block";
        //$('RemoveImgBtn').show();
    }
}*/
/*
function imgChange()
{
    let input = document.getElementById("InputImg"); 
    var curFiles = input.files;
    if (curFiles.length !== 0) {
        if (validFileSize(curFiles[0])) {
            if (validFileType(curFiles[0])) {

                document.getElementById("UploadedImg").src = window.URL.createObjectURL(curFiles[0]);
                document.getElementById("UploadedImg").style.display = 'block';
                //document.getElementById("RemoveImgBtnAddModal").style.display = "block";
                document.getElementById("fileName").textContent = curFiles[0].name;
                imageCahanged = true;
            }
            else {
                ShowModalAlert("Wrong file type")
                document.getElementById('InputImg').value = null;
            }
        }
        else {
            ShowModalAlert(`File is too big, maximum file size = ${returnMaximumFileSize()}`);
            document.getElementById('InputImg').value = null;
        }
    }
}*/
function validFileSize(file) {
    return file.size < MaximumFileSize;
}
function validFileType(file) {
    for (var i = 0; i < fileTypes.length; i++) {
        if (file.type === fileTypes[i]) {
            return true;
        }
    }

    return false;
}
function inputClick()
{
    document.getElementById('InputImg').click();
}
/*
function RemoveImageFromModal() {
    dialogConfirm('Remove image?',
        function () {
            imageCahanged = "-1";
            document.getElementById('InputImg').value = null;
            document.getElementById("UploadedImg").src = "//:0";
            document.getElementById("UploadedImg").style.display = "none";
            // document.getElementById("DefaultImageRef").href;
        },
        function () { });
}*/
function ShowModalAlert(text) {
    document.getElementById('alertModalText').innerHTML = text;
    $('#alertModalWindow').modal('show')
}
function dialogConfirm(message, yesCallback, noCallback) {
    document.getElementById('dialogMessage').textContent = message;
    $('#confirmDialog').modal('show');
    $('#dialogBtnYes').click(function () {
        $('#confirmDialog').modal('hide');
        yesCallback();
    });
    $('#dialogBtnNo').click(function () {
        $('#confirmDialog').modal('hide');
        noCallback();
    });
}
function ShowAddPaymentModal()
{
    $('#modalTitle').text("Add payment method");
    checkCommisionFields();
    $('#deleteButtom').hide();
    $('#RecordDescription').hide();
    $('.bd-AddEdit-modal').modal('show');
}
function checkCommisionFields()
{
    if ($("#paymentType").val() == 0) {
        $(".commission").each(function () {
            this.style.display = 'none';
        });
    }
    else if ($("#paymentType").val() == null || $("#paymentType").val() == -1 || $("#paymentType").val() == undefined)
    {
        $(".commission").each(function () {
            this.style.display = 'block';
        });
    }
    else {
        $(".commission").each(function () {
            this.style.display = 'block';
        });
    }
}
function checkStackRequest() {
    if (stackRequest == 0)
        document.getElementById('progressBar').style.display = "none";
    else
        document.getElementById('progressBar').style.display = "block";
};
function clearModal()
{
    paymenttoEditId = -1;
    $('#MdName').val("");
    $('#inputCurency').val("");
    $('#paymentType').val("-1");
    $('#commission').val("");
    $('#mincommission').val("");
    $('#TrCommission').val("");
    $('#deleteButtom').show();
    $('#RecordDescription').show();
    $('.form-control').each(function () {
        $(this).removeClass('invalid--form-control');
    });
    $('.invalid-feedback').each(function () {
        this.style.display = 'none';
    });



    //document.getElementById('InputImg').value = null;
    //document.getElementById("UploadedImg").src = "//:0";
    //document.getElementById("UploadedImg").style.display = "none";
}
function saveChanges() {
    if (CheckModalFieldValid()) {
        let object = getObjectFromModal();
        updateRequest(object);
        UpdateModel(object);
        ModelOnView();
        $('.bd-AddEdit-modal').modal('hide');
    }
}
function searchByName(mask)
{
    removeAllElemFromView();
    let count = 0;
    for (let i = 0; i < model.length; i++)
    {
        if (model[i].PaymentName.toLowerCase().includes(mask.toLowerCase()))
        {
            AddObjectOnView(model[i]);
            count++;
            if (count >= PageSize)
                break;
        }
    }
    
}
function DeleteRecord()
{
    dialogConfirm("Are you really want to delete payment method?", deleteCallback, function () { })
}
function deleteCallback()
{
    removeFromModel();
    ModelOnView();
    updateRequest({
        PaymentId: paymenttoEditId,
        deleted: 1
    });
    modalClose();
}
function removeFromModel()
{
    for (let i = 0; i < model.length; i++)
    {
        if (model[i].PaymentId == paymenttoEditId) {
            model.splice(i, 1);
            break;
        }
    }
}
/*
function isAnyNewImage() {
    return document.getElementById('InputImg').value != null || document.getElementById('InputImg').value != undefined;
}
function isAnyImage()
{
    return document.getElementById("UploadedImg").src != null || document.getElementById("UploadedImg").src != undefined || document.getElementById("UploadedImg").src != "" || document.getElementById("UploadedImg").src != "//:0";
}
function getFileFromModal()
{
    return document.getElementById("InputImg").files[0];
}*/
function updateRequest(obj)
{
    stackRequest++;
    checkStackRequest();
    let formdata = new FormData();
   // formdata.append('jsonObj', JSON.stringify(obj));
    /*
    if (isAnyNewImage()) {
        formdata.append('Icon', getFileFromModal());
    }
    else
    {
        if (!isAnyImage)
        {
            formdata.append("noIcon",true)
        }
    }*/
    $.ajax({
        type: "POST",
        url: updateLink,
        data:
        {
            jsonObj: JSON.stringify(obj)
        },
        dataType: "json",
        async: true,
        success: function (result) {
            result = JSON.parse(result);
            if (result.ResultCode >= 0) {
                if (obj.PaymentId < 0) {
                    $(`#${obj.PaymentId}`).attr('id', result.Value)
                    for (let i = 0; i < model.length; i++)
                    {
                        if (model[i].PaymentId == obj.PaymentId)
                        {
                            model[i].PaymentId = result.Value;
                            break;
                        }
                    }
                }
            }
            stackRequest--;
            checkStackRequest();
        },
        error: errorAjax
    });
}
function ModelOnView()
{
    removeAllElemFromView();
    for (let i = currentPage * PageSize; i < (currentPage * PageSize + PageSize) && i < model.length; i++)
        AddObjectOnView(model[i]);
}
function removeAllElemFromView()
{
    $('#divPaymentList [name="Payment"]').remove();
}
function AddObjectOnView(obj)
{
    //paymentTemplate templateAlert
    let template = $("#paymentTemplate").contents().clone();
    template.find("a").attr('id', obj.PaymentId);
    template.find("[name='name']").html(obj.PaymentName);
    template.find("img").attr('src', $(`#PaymentTypeIcon${obj.PaymentType}`).attr('src'));
    template.appendTo($('#divPaymentList'));

}
function UpdateModel(obj)
{
    if (findPayTypeById(obj.PaymentId)) {
        for (let i = 0; i < model.length; i++)
            if (model[i].PaymentId == obj.PaymentId)
                model[i] = obj;
    }
    else {
        if (obj.PaymentId < 0) {
            obj.PaymentId = findNewId();
            model.unshift(obj);
        }
    }
    checkButton();
}
function showModal(id)
{
    paymenttoEditId = id;
    let current = findPayTypeById(id);
    $('#RecordDescription').text(`${current.PaymentName} (${current.PaymentId})`);
    $('#MdName').val(current.PaymentName);
    $('#inputCurency').val(current.CurrencyISO);
    $('#paymentType').val(current.PaymentType);
    $('#commission').val(current.ComissionPCT);
    $('#mincommission').val(current.MinimumComission);
    $('#TrCommission').val(current.TransactionComission);
    $('.bd-AddEdit-modal').modal('show');
    checkCommisionFields();
}
function findPayTypeById(id)
{
    for (let i = 0; i < model.length; i++)
        if (id == model[i].PaymentId)
            return model[i];
}
function modalClose()
{
    $('.bd-AddEdit-modal').modal('hide');
}
function getObjectFromModal()
{
    return{
        PaymentId: paymenttoEditId,
        PaymentName: $('#MdName').val(),
        PaymentType: $('#paymentType').val(),
        CurrencyISO: $('#inputCurency').val(), 
        ComissionPCT: $('#commission').val(), 
        MinimumComission: $('#mincommission').val(),
        TransactionComission: $('#TrCommission').val(),
    };
}
function errorAjax(jqXHR, exception) {
    var msg = '';
    if (jqXHR.status === 0) {
        msg = 'Not connect.\n Verify Network.';
    } else if (jqXHR.status == 404) {
        msg = 'Requested page not found. [404]';
    } else if (jqXHR.status == 500) {
        msg = 'Internal Server Error [500].';
    } else if (exception === 'parsererror') {
        msg = 'Requested JSON parse failed.';
    } else if (exception === 'timeout') {
        msg = 'Time out error.';
    } else if (exception === 'abort') {
        msg = 'Ajax request aborted.';
    } else {
        msg = 'Uncaught Error.\n' + jqXHR.responseText;
    }
    stackRequest--;
    checkStackRequest();
    showBadResult(msg);
}
function showBadResult(msg)
{
    let template = $("#templateAlert").contents().clone();
    template.find('[name="text"]').html(msg);
    template.prependTo(document.getElementById("mainDiv"));
}
function findNewId() {
    let id = -1;
    for (let i = 0; i < model.length; i++) {
        if (model[i].PaymentId == id) {
            id--;
            i = 0;
            continue;
        }
    }
    return id;
}
function checkButton()
{
    if (currentPage != 0)
    {
        $('#btnPrevious').prop('disabled', false);
    }
    else 
        $('#btnPrevious').prop('disabled', true);
    if (model.length > (currentPage + 1) * PageSize )
    {
        $('#btnNext').prop('disabled', false);
    }
    else 
        $('#btnNext').prop('disabled', true);
}
function PreviousRecords()
{
    currentPage--;
    ModelOnView();
    checkButton();
}
function NextRecords()
{
    currentPage++;
    ModelOnView();
    checkButton();
}
function returnMaximumFileSize() {

    if (MaximumFileSize < 1024) {
        return MaximumFileSize + 'bytes';
    } else if (MaximumFileSize > 1024 && MaximumFileSize < 1048576) {
        return (MaximumFileSize / 1024).toFixed(1) + 'KB';
    } else if (MaximumFileSize >= 1048576) {
        return (MaximumFileSize / 1048576).toFixed(1) + 'MB';
    }
}
/*
   MdName
   inputCurency
   paymentType
   commission
   mincommission
   TrCommission
 */ 
function CheckModalFieldValid()
{
    let ret = true;
    if ($("#MdName").val().length == 0) {
        $("#MdName").addClass("invalid--form-control");
        ret = false;
    }
    else
    {
        $("#MdName").removeClass("invalid--form-control");
    }
    if (!$("#inputCurency option:selected").is('[value]')) {
        $("#inputCurency").addClass("invalid--form-control");
        ret = false;
    }
    else
    {
        $("#inputCurency").removeClass("invalid--form-control");
    }
    if (!$("#paymentType option:selected").is('[value]')) {
        $("#paymentType").addClass("invalid--form-control");
        ret = false;
    }
    else
    {
        $("#paymentType").removeClass("invalid--form-control");
    }

    if (!$("#paymentType").val() != 0) {
        if ($("#commission").val().length == 0 || !isNumeric($("#commission").val())) {
            $("#commission").addClass("invalid--form-control");
            if (!isNumeric($("#commission").val())) {
                document.getElementById("invComm").style.display = 'block';
            }
            ret = false;
        }
        else {
            $("#commission").removeClass("invalid--form-control");
            document.getElementById("invComm").style.display = 'none';
        }

        if ($("#mincommission").val().length == 0 || !isNumeric($("#mincommission").val())) {
            $("#mincommission").addClass("invalid--form-control");
            if (!isNumeric($("#mincommission").val())) {
                document.getElementById("invMinComm").style.display = 'block';
            }
            ret = false;
        }
        else {
            $("#mincommission").removeClass("invalid--form-control");
            document.getElementById("invMinComm").style.display = 'none';
        }
        if ($("#TrCommission").val().length == 0 || !isNumeric($("#TrCommission").val())) {
            $("#TrCommission").addClass("invalid--form-control");
            if (!isNumeric($("#TrCommission").val())) {
                document.getElementById("invTrnComm").style.display = 'block';
            }
            ret = false;
        }
        else {
            $("#TrCommission").removeClass("invalid--form-control");
            document.getElementById("invTrnComm").style.display = 'none';
        }
    }
    return ret;
}
function isNumeric(str) {
    if (typeof str != "string") return false 
    return !isNaN(str) &&
        !isNaN(parseFloat(str)) 
}