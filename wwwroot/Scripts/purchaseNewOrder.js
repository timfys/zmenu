var catalogData;
var cartData = [];
var catalogPage = 1;

$(document).ready(function($) {
    LoadCatalog();

    createCartTable();

    $("#CatalogPrevious").click(function () {
        if (!$(this).hasClass('disabled')) {
            catalogPage--;
            LoadCatalog();
        }
    });

    $("#CatalogNext").click(function() {
        if (!$(this).hasClass('disabled')) {
            catalogPage++;
            LoadCatalog();
        }
    });

    $('#editCartModal').on('shown.bs.modal', function () {
        $('#EditCartRemarkToSupplier').trigger('focus');
    });

    $('#EditCartRemarkToSupplier').keypress(function (event) {
        var keycode = event.keyCode ? event.keyCode : event.which;
        if (keycode == '13') {
            btnSaveEditCart.click();
        }
    });
});

function LoadCatalog() {
    $("#catalogTable").html("");
    $("#CatalogPagination").addClass("d-none");
    $("#CatalogLoading").removeClass("d-none");

    $("#CatalogSearch").attr("disabled", "disabled");
    $("#CatalogPerPage").attr("disabled", "disabled");

    $("#CatalogPrevious").addClass("disabled");
    $("#CatalogNext").addClass("disabled");

    $("#CatalogPageNumber").html(catalogPage);

    $.ajax({
        type: "GET",
        url: "/Purchases/Catalog",
        data: {
            page: catalogPage,
            perPage: $("#CatalogPerPage").val(),
            search: $("#CatalogSearch").val(),
            supplier: $("#SelectSupplier").val()
        },
        success: function (data) {
            console.log(data);
            catalogData = data.DecryptResponse;

            if (catalogPage > 1) {
                $("#CatalogPrevious").removeClass("disabled");
            }
            if (catalogData.length - 1 == $("#CatalogPerPage").val()) {
                catalogData.splice(-1, 1);
                $("#CatalogNext").removeClass("disabled");
            }

            createCatalogTable(catalogData);
            LoadingEnd();

            $("#CatalogPagination").removeClass("d-none");
            $("#CatalogLoading").addClass("d-none");
            $("#CatalogSearch").removeAttr("disabled", "disabled");
            $("#CatalogPerPage").removeAttr("disabled", "disabled");
        }
    });
}

function PlaceOrder() {
    $("#ResultPlaceOrder").html(
        $("<p />",
            {
                html: "Placing order...",
                'class': "alert alert-warning",
                role: "alert"
            }).append(
            $("<button />",
                {
                    type: "button",
                    'class': "close",
                    'data-dismiss': "alert",
                    'aria-label': "Close"
                }).append(
                $("<span />",
                    {
                        'aria-hidden': "true",
                        html: "&times;"
                    })
            )
        ));

    $.ajax({
        type: "POST",
        url: "/Purchases/PlaceOrder",
        data: {
            purchaseProductOrders: cartData
        },
        success: function (response) {
            console.log(response);
            if (response.Success) {
                $("#ResultPlaceOrder").html(
                    $("<p />",
                        {
                            html: response.DecryptResponse.ResultMessage,
                            'class': "alert alert-success",
                            role: "alert"
                        }).append(
                        $("<button />",
                            {
                                type: "button",
                                'class': "close",
                                'data-dismiss': "alert",
                                'aria-label': "Close"
                            }).append(
                            $("<span />",
                                {
                                    'aria-hidden': "true",
                                    html: "&times;"
                                })
                        )
                    ));

                cartData = [];
                cartTableReload(cartData);
                catalogPage = 1;
                LoadCatalog();
            }
            else if (response.Message) {
                $("#ResultPlaceOrder").html(
                    $("<p />",
                        {
                            html: response.Message,
                            'class': "alert alert-danger",
                            role: "alert"
                        }).append(
                        $("<button />",
                            {
                                type: "button",
                                'class': "close",
                                'data-dismiss': "alert",
                                'aria-label': "Close"
                            }).append(
                            $("<span />",
                                {
                                    'aria-hidden': "true",
                                    html: "&times;"
                                })
                        )
                    ));
            }
            //if (response.Success) {
            //    window.location.href = "/Warehouse/Purchases";
            //}
        }
    });
}

function AddToCart(productId, purchaseProductId) {
    var cartItem = Enumerable.From(catalogData).
        Where(x => x.ProductId == productId && x.Purchase_ProductId == purchaseProductId).FirstOrDefault();
    if (cartItem) {
        cartData.push(cartItem);
        // removeFromCatalog(productId, purchaseProductId);
        catalogTableReload(catalogData);
        cartTableReload(cartData);
    }
}

function removeFromCatalog(productId, purchaseProductId) {
    for(var i = 0; i < catalogData.length; i++){ 
        if (catalogData[i].ProductId == productId && catalogData[i].Purchase_ProductId == purchaseProductId) {
            if (parseInt(catalogData[i].QtyInStock)) {
                //catalogData[i].QtyInStock--;
            }
            return;
        }
    }
}

function AddToCatalog(productId, purchaseProductId) {
    for( var i = 0; i < catalogData.length; i++){ 
        if (catalogData[i].ProductId == productId && catalogData[i].Purchase_ProductId == purchaseProductId) {
            if (parseInt(catalogData[i].QtyInStock)) {
                //catalogData[i].QtyInStock++;
            }
            return;
        }
    }
}

function EditCart(productId, purchaseProductId, val) {
    for( var i = 0; i < cartData.length; i++){ 
        if (cartData[i].ProductId == productId && cartData[i].Purchase_ProductId == purchaseProductId) {
            cartData[i].Remark_To_Supplier = val;
            cartTableReload(cartData);
            return;
        }
    }
}

function RemoveFromCart(productId, purchaseProductId) {
    for( var i = 0; i < cartData.length; i++){ 
        if (cartData[i].ProductId == productId && cartData[i].Purchase_ProductId == purchaseProductId) {
            cartData.splice(i, 1);
            // AddToCatalog(productId, purchaseProductId);
            cartTableReload(cartData);
            catalogTableReload(catalogData);
            return;
        }
    }
}
    