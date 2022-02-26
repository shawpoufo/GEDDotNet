$(document).ready(function () {
    //document.getElementById('FolderPrivilegeForm').addEventListener('click', event => event.preventDefault())
    let validateUserId = (newUserId) => {
        if (newUserId === null || newUserId === undefined || newUserId === "") {
            document.getElementById('newUserIdMessage').innerText = 'veuiller choisire un utilisateur'
            return false
        }
        
        document.getElementById('newUserIdMessage').innerText = ""
        return true
    }

    let validateChoice = (read, write, download) => {
        let divChoiceMessage = document.getElementById('choiceMessage')
        if (!read && !write && !download) {
            divChoiceMessage.innerHTML = "veuiller faire un choix"
            return false
        }
        else if (download && !read) {
            divChoiceMessage.innerHTML = "pour télécharger il doit y'avoir le droit de lire"
            return false
        }
        else if (write && !read) {
            divChoiceMessage.innerHTML = "pour modifier il doit y'avoir le droit de lire"
            return false
        }
        else
        {
            divChoiceMessage.innerHTML = ""
            return true
        }
            
    }
    const getUrl = () => {
        const docType = document.getElementById('doc-type').value;
        const formState = document.getElementById('form-state').value;
        let url = document.getElementById('privilege-form').getAttribute('action');
        if (docType === 'document' && formState === 'update') {
                url = '/DocumentPrivilege/update'
        }
        else
        {
            if (formState === 'update') 
                url = '/FolderPrivilege/update';
        }
        return url;
    }

    const clear = () => {
        document.getElementById('read').checked = false;
        document.getElementById('write').checked = false;
        document.getElementById('download').checked = false;
        $('#select-search').val(null).trigger('change');
        $('#form-state').val('new');
        document.getElementById('newUserId').value = "";
        $('#btnPrivilegeAdd').html('ajouter')
        $("#btn-cancel-update").css("display", "none")


    }
    function fnSubmit(e) {
        const path = document.getElementById('path').value
        const newUserIdValue = document.getElementById('newUserId').value
        const readValue = document.getElementById('read').checked
        const writeValue = document.getElementById('write').checked
        const downloadValue = document.getElementById('download').checked
        const privilegeStrategy = Array.from(document.getElementsByName('rb-privilege-strategy')).filter(r => r.checked)[0].value

        if (validateUserId(newUserIdValue) && validateChoice(readValue, writeValue, downloadValue)) {
            $.ajax({
                url: getUrl(),
                method: 'POST',
                data: {
                    NewUserId: newUserIdValue,
                    Read: readValue,
                    Write: writeValue,
                    DownLoad: downloadValue,
                    Path: path,
                    PrivilegeStrategy: privilegeStrategy
                },
                success: function (resp) {
                    document.getElementById('privilegeList').innerHTML = resp 
                    clear()
                },
                error: function (error) {
                    console.log("error")
                }
            })
        }
    }
    document.getElementById("btnPrivilegeAdd").addEventListener("click", fnSubmit)
    document.getElementById("btn-cancel-update").addEventListener("click", (e) => {
        clear()

    })

});