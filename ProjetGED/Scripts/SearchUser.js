$(document).ready(function () {
    let config = {
        dropdownParent: $('#manageView'),
        ajax: {
            url: $('#search-script').attr('data-url'),
            data: function (parameter) {
                let query = {
                    userName: parameter.term

                }
                return query;
            }
        }
    }
    $("#select-search").select2(config);
    $("#select-search").on("change", function (par) {
        let selected = $("#select-search").select2('data')[0]
        if (selected) {
            $('#newUserId').val(selected.id)
            $('#newUserIdMessage').text("")
        }
    })
    $("#manageView").on("shown.bs.modal", () => {
        console.log("hi");
    })
    $("#manageView").on("hidden.bs.modal", () => {
        console.log("bye");
    })
});