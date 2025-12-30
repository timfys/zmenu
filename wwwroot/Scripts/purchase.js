var catalogData;
var cartData = [];

$(document).ready(function($) {
    LoadingStart();

    $.get("/Purchases/Catalog",
        function (data) {
            catalogData = data;
            createCatalogTable(catalogData);
            LoadingEnd();
        });


    createCartTable();

    $('#editCartModal').on('shown.bs.modal', function () {
      $('#RemarkToSupplier').trigger('focus')
    });

    $('#RemarkToSupplier').keypress(function(event){
	    var keycode = (event.keyCode ? event.keyCode : event.which);
	    if(keycode == '13'){
		    btnSaveEditCart.click();
	    }
    });

});

function AddToCart(id) {
    cartItem = Enumerable.From(catalogData).
            Where(x => x.Id == id ).FirstOrDefault();
    if (cartItem) {
        cartData.push(cartItem);
        removeFromCatalog(id);
        catalogTableReload(catalogData);
        cartTableReload(cartData);
    }
}

function removeFromCatalog(id) {
    for( var i = 0; i < catalogData.length; i++){ 
        if (catalogData[i].Id == id) {
            catalogData[i].Avaliable -=1;
            return;
        }
    }
}

function AddToCatalog(id) {
    for( var i = 0; i < catalogData.length; i++){ 
        if (catalogData[i].Id == id) {
            catalogData[i].Avaliable +=1;
            return;
        }
    }
}

function EditCart(id, val) {
    for( var i = 0; i < cartData.length; i++){ 
        if (cartData[i].Id == id) {
            cartData[i].RemarkToSupplier = val;
            cartTableReload(cartData);
            return;
        }
    }
}

function RemoveFromCart(id) {
    for( var i = 0; i < cartData.length; i++){ 
        if (cartData[i].Id == id) {
            cartData.splice(i, 1);
            AddToCatalog(id);
            cartTableReload(cartData);
            catalogTableReload(catalogData);
            return;
        }
    }
}
    