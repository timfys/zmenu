$('#forgot-pass').click(async function () {


    let userName = document.getElementById('regmobile');
    let countryCode= document.getElementById("countrycode").value.substring(1);
    if (!userName.value) {
        userName.classList.add('has-error');
        return;
    } else
        userName.classList.remove('has-error');

    let resp = await fetch(`/JoinBusiness/ForgotPassword/phone=${userName.value}`,
        {
            method: "POST",
            headers :
                {
                    "CountryPrefix" : countryCode
                }
            })

    document.querySelector("#form-valid").style.display = "block";

    let respBody = await resp.json();
    if (resp.ok) {

        document.querySelector("#ok").style.display = "none";
        document.querySelector("#error").style.display = "none";
        if(userName[0] === "0"){
            userName = userName.substring(1);
        }
        document.querySelector("#sms_sent").textContent = document.querySelector("#sms_sent").textContent.replace("{}", `${countryCode}${userName.value}`);
        document.querySelector("#sms_sent").style.display = "block";
        //document.querySelector(".ListError").innerHTML = `<li style='color: green'>${respBody.resultMessage}</li>`
    } else {
        document.querySelector("#sms_sent").style.display = "none";
        document.querySelector("#ok").style.display = "none";
        document.querySelector("#error").style.display = "block";
        document.querySelector("#error").textContent = `${respBody.resultMessage}`;
    }
});
function setInputFilter(textbox, inputFilter) {
    ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach(function (event) {
        textbox.addEventListener(event, function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    });
}


//Maybe something is not needed((()))
$(document).ready(function () {
    setInputFilter(document.getElementById("regmobile"), function (value) {
        return /^\d*$/.test(value) && value.length < 30;
    });
    $("#login").click(async function (e) {
        e.preventDefault();

        let username = document.querySelector("#regmobile");
        
        let password = document.querySelector("#pwd");
        
        if(!ValidateValues([username, password]))
            return;

        ShowProgress();

        let resp = await fetch("/JoinBusiness/LoginCrm",
            {
                method : "POST",
                headers :
                    {
                        "Content-Type" : "application/json",
                        "CountryPrefix" : document.querySelector("#countrycode").value.substring(1)
                    },
                body : JSON.stringify(
                    {
                        Username : username.value,
                        Password : password.value,
                        EntityId : 0
                    })
            })


        if(resp.ok)
        {
            let langIso = document.cookie.split("current_culture=")[1].split(";")[0];
            if(langIso === "")
                langIso = "en"
            let lid = await resp.text();
            window.location.href = `https://crm.menu4u.tech/restaurant/${langIso}/menus?lid=` + lid;
        }else if(resp.status === 403)
        {
            window.location.href = resp.headers.get("LastStepUrl");
        }
        else if(resp.status === 400 || resp.status === 404)
        {
            let respBody = await resp.json();
            let nodes = document.querySelectorAll(".errorLi");

            if (nodes.length >= 1) {
                nodes.forEach(x => x.remove());
            }
            document.querySelector("#form-valid").style.display = "block";
            document.querySelector(".ListError").style.display = "block";
            document.querySelector("#ok").style.display = "none";
            document.querySelector("#sms_sent").style.display = "none";
            let errorNode = document.querySelector("#error");
            errorNode.textContent = respBody.resultMessage;
            errorNode.style.display = "block";
            errorNode.focus();

        }
        
        HideProgress();
        
        
    });
    $("#register").click(async function (e) {
        e.preventDefault();

        let mobile = document.querySelector("#regmobile");

        let fullName = document.querySelector("#regname");

        let email = document.querySelector("#exampleInputEmail1");

        let password = document.querySelector("#password");

        let passwordConfirm = document.querySelector("#password_2");

        if (passwordConfirm.value != password.value)
            return;

        if (!ValidateValues([mobile, fullName, email, password, passwordConfirm]))
            return;

        let request =
            {
                Email: email.value,
                FirstName: $("#regname").val().split(" ")[0],
                LastName: $("#regname").val().split(" ")[1] == undefined ? "" : $("#regname").val().split(" ")[1],
                Mobile: mobile.value,
                Password: password.value,
                CountryISO: $("#country").val(),
                ConfirmPassword: passwordConfirm.value,
                Culture: window.localStorage.getItem("iso"),
                OrderProductId: getQueryString('id') == undefined ? 1 : parseInt(getQueryString('id')),
            };

        //debugger;
        if (!$('#agree').is(":checked")) {
            alert('Please accept terms and conditions!!!');
        } else {
            document.querySelectorAll("#frmLogin .has-error").forEach(function (input) {
                input.classList.remove('has-error')
            });
            ShowProgress();

            let resp = await fetch("/JoinBusiness/CRegister", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(request)
            })

            if (resp.ok) {
                let respBody = await resp.json();

                if ("entityId" in respBody) {
                    let ticketRequest =
                        {
                            Username: respBody.username,
                            Password: respBody.password,
                            EntityId: respBody.entityId
                        }
                    await CreateTicketRequest()

                } else {
                    HideProgress();
                    const ListError = document.querySelector('#form-valid .ListError');
                    ListError.innerHTML = '';
                    ListError.parentNode.style.display = "block";

                    PrintErrorsFromModelState(respBody, ListError);
                }
            } else {

                let respBody = await resp.json();

                if (Math.abs(respBody.resultCode) == 5674) {
                    document.querySelector("#checkPass1").checked = true;
                    checkPass(1);
                }

                let nodes = document.querySelectorAll(".errorLi");

                if (nodes.length >= 1) {
                    nodes.forEach(x => x.remove());
                }
                document.querySelector("#form-valid").style.display = "block";
                document.querySelector(".ListError").style.display = "block";
                document.querySelector("#ok").style.display = "none";
                document.querySelector("#sms_sent")
                let errorNode = document.querySelector("#error");
                errorNode.textContent = respBody.resultMessage;
                errorNode.style.display = "block";
                errorNode.focus();
                
                HideProgress();
                return false;
            }

        }

    });

    function ValidateValues(domObjectsArray) {
        let isValid = true;
        let elemInScroll = false;
        
        for (let i = 0; i < domObjectsArray.length; i++) {
            if (domObjectsArray[i].value == '') {
                domObjectsArray[i].classList.add("customWarning");
                if(!elemInScroll){
                    domObjectsArray[i].focus();
                    elemInScroll = true;
                }
                isValid = false;
            } else {
                domObjectsArray[i].classList.remove("customWarning");
            }
        }

        return isValid;
    }

    async function CreateTicketRequest() {

        let iso = window.localStorage.getItem("iso");
        ShowProgress();

        let resp = await fetch("/JoinBusiness/CreateVerifyTicket", {
            method: "POST",
        })

        let respBody = await resp.json();
        
        HideProgress();
        if (resp.ok) {
            window.location.href =  iso === 'en' ? `/login/phoneconfirm` : `/${iso}/login/phoneconfirm`
        } else {
        }
    }

    function PrintErrorsFromModelState(data, elementToInsert, IsSignIn) {
        for (item of data) {
            if ("ResultCode" in item) {

                elementToInsert.insertAdjacentHTML("beforeend", `<li>${item.ResultMessage}</li>`)


                switch (item.ResultCode) {
                    case -811: {
                        let txtCity = document.querySelector('#txtCity');
                        txtCity.classList.remove('valid');
                        txtCity.classList.add('has-error');
                        break;
                    }
                    case -812: {
                        let txtAddress = document.querySelector('#txtAddress');
                        txtAddress.classList.remove('valid');
                        txtAddress.classList.add('has-error');
                        break;
                    }
                    case -813: {
                        let txtZip = document.querySelector('#txtZip');
                        txtZip.classList.remove('valid');
                        txtZip.classList.add('has-error');
                        break;
                    }
                    case -911: {
                        let exapleInputEmail = document.querySelector('#exampleInputEmail1');
                        exapleInputEmail.classList.remove('valid');
                        exapleInputEmail.classList.add('has-error');
                        break;
                    }
                    case -912: {
                        let regname = document.querySelector('#regname');
                        regname.classList.remove('valid');
                        regname.classList.add('has-error');
                        break;
                    }
                    case -913: {
                        let regmobile = document.querySelector('#regmobile');
                        regmobile.classList.remove('valid');
                        regmobile.classList.add('has-error');
                        break;
                    }
                    case -914: {
                        let pwd = document.querySelector('#pwd');
                        let password = document.querySelector('#password');

                        IsSignIn ?
                            pwd.classList.remove('valid') :
                            password.classList.remove('valid');

                        IsSignIn ?
                            pwd.classList.add('has-error') :
                            password.classList.add('has-error');
                        break;
                    }
                    case -915: {
                        let password = document.querySelector('#password');
                        let password_2 = document.querySelector('#password_2');

                        password.classList.remove('valid');
                        password_2.classList.remove('valid');

                        password.classList.add('has-error');
                        password_2.classList.add('has-error');
                        break;
                    }
                    case -916: {
                        $('#country').removeClass('valid');
                        $('#country').addClass('has-error');
                        break;
                    }
                }

            } else {
                window.location.replace(data.ReturnUrl);
                break;
            }
        }
    }

    function hasErrors(formSelector, inputSelector) {
        var hasErrors = false;
        var form = document.querySelector(formSelector);
        form.querySelectorAll(inputSelector).forEach(function (input) {
            if (input.value == "")
                input.classList.add('has-error');
            else
                input.classList.remove('has-error');

            if (input.classList.contains('has-error')) hasErrors = true;
        });
        return hasErrors;
    }

    $("#btnship").click(function () {
        if (!hasErrors("#frmAddress", 'input[type="text"]')) {

            ShowProgress();
            var culture = window.localStorage.getItem("iso");
            culture = (culture == "he") ? "" : "/" + culture;

            $.ajax({
                url: '/umbraco/Surface/Confirmation/ConfirmationStep2',
                data: "{'city': '" + $("#txtCity").val() + "','address':'" + $("#txtAddress").val() + "','zipcode':'" + $("#txtZip").val() + "', 'culture': '" + window.localStorage.getItem("iso") + "' }",
                contentType: "application/json; charset=utf-8",
                type: 'POST',
                dateType: "Json",
                success: function (data) {

                    if ("ReturnUrl" in data) {
                        window.location.replace(data.ReturnUrl);
                    } else {
                        HideProgress();
                        const ListError = document.querySelector('#form-valid .ListError');
                        ListError.innerHTML = '';
                        ListError.parentNode.style.display = "block";

                        PrintErrorsFromModelState(data, ListError, true);
                    }
                },
                failure: function (msg) {
                    HideProgress();
                }
            });
        }
        return false;
    });


    $("#change-phone-verify").click(function () {

        var mobile = $("#InputPhone").val();
        if ($("#frmVerify").valid()) {
            ShowProgress();
            $.ajax({
                url: '/umbraco/Surface/Login/VerifyPhone',
                data: "{'mobile':'" + mobile + "', 'culture': '" + window.localStorage.getItem("iso") + "'}",
                contentType: "application/json; charset=utf-8",
                type: 'POST',
                dateType: "Json",
                success: function (data) {
                    if (data.Url != "") {
                        redirectToProfile();
                        // window.location.replace(data.Url);
                        HideProgress();
                    } else {
                        var obj = JSON.parse(data);
                        if (obj == "-1") {
                            HideProgress();
                            alert("Customer exists with this e-mail!!!");
                            $("#diverrorregister").css("display", "block");

                        }
                    }

                },
                failure: function (msg) {
                    HideProgress();
                }
            });

            return false;
        }

    });

    function redirectToProfilewithoutverify(uname, pass) {
        $.ajax({
            url: '/umbraco/Surface/Login/ProfilePage',
            data: "{'username': '" + uname + "','pwd':'" + pass + "'}",
            contentType: "application/json; charset=utf-8",
            type: 'POST',
            dateType: "Json",
            success: function (data) {
                window.location.replace(data.Url);
            },
            failure: function (msg) {
            }
        });
    }

    function redirectToProfieUrl() {
        $.ajax({
            url: '/umbraco/Surface/Login/UrlProfilePage',
            data: "",
            contentType: "application/json; charset=utf-8",
            type: 'POST',
            dateType: "Json",
            success: function (data) {
                window.location.replace(data.Url);
            },
            failure: function (msg) {
            }
        });
    }

    function redirectToProfile() {
        var token = window.localStorage.getItem("token");
        var gid = getQueryString('id');
        $.ajax({
            url: '/umbraco/Surface/Login/ProfilePage',
            data: "{'token': '" + token + "','orderProductId':'" + gid + "'}",
            contentType: "application/json; charset=utf-8",
            type: 'POST',
            dateType: "Json",
            success: function (data) {
                if (data != "") {
                    window.location.replace(data.Url);
                }
            },
            failure: function (msg) {
                HideProgress();
            }
        });
    }

    function redirectToUpdatePage() {

        $.ajax({
            url: '/umbraco/Surface/Login/UpdatePage',
            data: "",
            contentType: "application/json; charset=utf-8",
            type: 'POST',
            dateType: "Json",
            success: function (data) {
                window.location.replace(data.Url);
            },
            failure: function (msg) {
            }
        });

    }

    function redirectToVerifyPage() {

        $.ajax({
            url: '/umbraco/Surface/Login/VerifyPage',
            data: "",
            contentType: "application/json; charset=utf-8",
            type: 'POST',
            dateType: "Json",
            success: function (data) {
                window.location.replace(data.Url);
            },
            failure: function (msg) {
            }
        });

    }

    var modal, loading;

    function ShowProgress() {
        modal = document.createElement("DIV");
        modal.className = "modal";
        document.body.appendChild(modal);
        loading = document.getElementsByClassName("loading")[0];
        loading.style.display = "block";
        var top = Math.max(window.innerHeight / 2 - loading.offsetHeight / 2, 0);
        var left = Math.max(window.innerWidth / 2 - loading.offsetWidth / 2, 0);
        loading.style.top = top + "px";
        loading.style.left = left + "px";
        $("#login").prop("disabled", true);
        $("#register").prop("disabled", true);
        $("input").attr('disabled', 'disabled');
        $("button").attr('disabled', 'disabled');
        $("a").attr('disabled', 'disabled');

    }

    function HideProgress() {
        document.body.removeChild(modal);
        loading.style.display = "none";
        $("#login").prop("disabled", false);
        $("#register").prop("disabled", false);
        $("input").removeAttr('disabled');
        $("button").removeAttr('disabled');
        $("a").removeAttr('disabled');
    }
});

function changeemailverify() {
    var uname = $("#InputEmail1").val();


    if ($("#frmVerify").valid()) {

        $.ajax({
            url: '/umbraco/Surface/Login/VerifyEmail',
            data: "{'Email': '" + uname + "', 'culture': '" + window.localStorage.getItem("iso") + "'}",
            contentType: "application/json; charset=utf-8",
            type: 'POST',
            dateType: "Json",
            success: function (data) {
                //var obj = JSON.val(data);
                if (data != "") {
                    alert("Successfully Registerd!!!")
                    localStorage.setItem('token', data);
                    redirectToUpdatePage();
                    // window.location.replace(data.Url);
                    HideProgress();
                } else {
                    HideProgress();
                    alert("Customer exists with this e-mail!!!");
                    $("#diverrorregister").css("display", "block");
                }
            },
            failure: function (msg) {
                HideProgress();
            }
        });

        return false;
    } else {
        alert("No");
    }
}

function getListGamePlaystation3(x) {
    $.ajax({
        url: '/umbraco/Surface/Index/LoadPlaystation3',
        data: "",
        contentType: "application/json; charset=utf-8",
        type: 'GET',
        dateType: "Json",
        success: function (data) {
            if (data != null) {
                var p3 = data;
                localStorage.setItem('divPlaystation3', p3);
                localStorage.setItem('values', x);
            } else {

                console.log('yes');
            }
        },
        failure: function (msg) {
        }
    });
}

function getListGameXBoxOne(x) {

    $.ajax({
        url: '/umbraco/Surface/Index/LoadXBoxOne',
        data: "",
        contentType: "application/json; charset=utf-8",
        type: 'GET',
        dateType: "Json",
        success: function (data) {
            if (data != null) {
                var p1 = data;
                localStorage.setItem('XBoxOne', p1);
                localStorage.setItem('values', x);
            } else {

                console.log('yes');
            }
        },
        failure: function (msg) {
        }
    });
}

function getListGameXBx360(x) {

    $.ajax({
        url: '/umbraco/Surface/Index/LoadXBx360',
        data: "",
        contentType: "application/json; charset=utf-8",
        type: 'GET',
        dateType: "Json",
        success: function (data) {
            if (data != null) {
                var p6 = data;
                localStorage.setItem('XBx360', p6);
                localStorage.setItem('values', x);
            } else {

                console.log('yes');
            }
        },
        failure: function (msg) {
        }
    });
}

function getListGameNintendoSwitch(x) {

    $.ajax({
        url: '/umbraco/Surface/Index/LoadNintendoSwitch',
        data: "",
        contentType: "application/json; charset=utf-8",
        type: 'GET',
        dateType: "Json",
        success: function (data) {
            if (data != null) {
                var p7 = data;
                localStorage.setItem('NintendoSwitch', p7);
                localStorage.setItem('values', x);
            } else {

                console.log('yes');
            }
        },
        failure: function (msg) {
        }
    });
}

$("#My-account").click(function () {
    var token = window.localStorage.getItem("token");
    var gid = getQueryString('id');
    $.ajax({
        url: '/umbraco/Surface/Login/MyAccount',
        data: "{'token': '" + token + "','orderProductId':'" + gid + "'}",
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        dateType: "Json",
        success: function (data) {
            if (data.url != null) {
                window.location.replace(data.Url);
            } else {
                localStorage.setItem('productId', data);
                window.localStorage.removeItem("orderProductId");
                redirectToProfieUrl();
            }
        },
        failure: function (msg) {
        }
    });
});

function ShowRentalProduct1() {
    var token = window.localStorage.getItem("token");

    $.ajax({
        url: '/umbraco/Surface/Login/ShowRentalProduct',
        data: "{'token': '" + token + "'}",
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        dateType: "Json",
        success: function (data) {
            if (data != null) {
                var returnedData = JSON.parse(data);
                $.each(returnedData, function (index, value) {
                    var image = "http://www.zone51.co.il/media/" + returnedData[index].inventoryId + "/Media/Images/FrontImage.jpg";
                    var pname = returnedData[index].productName;
                    var mySecondDiv = $('<div class="row py-4 border-bottom shipped" ><div class="col-12 col-sm-auto d-flex justify-content-center pr-sm-0 pb-3 pb-sm-0"><a class="mr-2" href="@Umbraco.NiceUrl(1152)?id=' + returnedData[index].inventoryId + '" target=_blank><img src=' + image + '></a></div><div class="col-12 col-sm col-lg-8 d-flex flex-column"><p class="h5 h4-sm h3-md">' + pname + '</p><p class="small h6-md text-muted mt-auto">Rented from <strong>10.01.2019</strong> to <strong>10.02.2019</strong></p></div></div>');
                    $("#image").append(image);
                    $("#MyRental").append(mySecondDiv);
                    console.log(mySecondDiv);
                });
            } else {
                alert("No");
            }
        },
        failure: function (msg) {
        }
    });
}

function RentBtn111(ID) {

    var token = window.localStorage.getItem("token");
    window.localStorage.setItem("orderProductId", ID);
    $.ajax({
        url: '/umbraco/Surface/Login/ShowRentals',
        data: "{'token': '" + token + "','ID':'" + ID + "'}",
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        dateType: "Json",
        success: function (data) {
            console.log(data);
            if (data == "1") {
                alert("Successfully Registerd!!!");
            } else if (data == "0") {
                //query string id for login page
                redirectToLoginForGetRent(ID);
            } else {
                alert("Try Again");
            }
        },
        failure: function (msg) {

        }
    });
}

function redirectToLoginForGetRent(ID) {
    var token = window.localStorage.getItem("token");
    $.ajax({
        url: '/umbraco/Surface/Login/ProfilePage',
        data: "{'token': '" + token + "','orderProductId':'" + ID + "'}",
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        dateType: "Json",
        success: function (data) {
            if (data.url != null) {
                window.location.replace(data.Url);
            } else {
                redirectToProfieUrlForGetRent(ID);
            }
        },
        failure: function (msg) {
        }
    });
}

function redirectToProfieUrlForGetRent(ID) {
    $.ajax({
        url: '/umbraco/Surface/Login/LoginPage',
        data: "",
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        dateType: "Json",
        success: function (data) {
            var lastId = "?id=" + ID;
            if (ID != null) {
                window.location.replace(data.Url + lastId);
            } else if (ID == null) {
                window.location.replace(data.Url);
            }
        },
        failure: function (msg) {

            HideProgress();
            $("#diverrorregister").css("display", "block");
        }
    });
}

function redirectToLogin() {

    $.ajax({
        url: '/umbraco/Surface/Login/LoginPage',
        data: "",
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        dateType: "Json",
        success: function (data) {
            window.location.replace(data.Url);
        },
        failure: function (msg) {
        }
    });

}

function getQueryString(name) {

    var url = window.location.href;

    name = name.replace(/[\[\]]/g, "\\$&");

    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);

    if (!results) return null;
    if (!results[2]) return '';

    return decodeURIComponent(results[2].replace(/\+/g, " "));
}


$("#frmAddress").validate({
    errorClass: 'help-block animation-slideDown', // You can change the animation class for a different entrance animation - check animations page
    errorElement: 'div',
    errorPlacement: function (error, e) {
        e.parents('.form-group > div').append(error);
    },
    highlight: function (e) {

        $(e).closest('.form-control').removeClass('has-success has-error').addClass('has-error');
        $(e).closest('.help-block').remove();
    },
    success: function (e) {
        e.closest('.form-control').removeClass('has-success has-error');
        e.closest('.help-block').remove();
    },
    rules: {
        'txtCity': {
            required: true
        },

        'txtAddress': {
            required: true
        },

        'txtZip': {
            required: true,
            number: true
        }
    },
    messages: {
        'txtCity': 'Please provide city',

        'txtAddress': 'Please provide your address',

        'txtZip': 'Please provide zip code'
    }

});


function makeRent(productID) {
    debugger;
    if (!productID) {
        try {
            productID = parseInt(window.location.href.split('/').reverse()[1]);
        } catch {
            throw "Can't get productID from queryString...";
            return;
        }
    }

    $.post("/Umbraco/Surface/ProfilePage/RentProduct", {
        productID: productID,
        culture: window.localStorage.getItem("iso")
    }, function (data) {
        debugger;
        window.location.replace(data.ReturnUrl);
    });
}



