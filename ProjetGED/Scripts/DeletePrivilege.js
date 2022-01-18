$('#privilegeList').on('click', function (e) {
    let element = e.target;
    if (element.classList.contains('btn-delete-privilege')) {
        const userEmail = $(element).attr('data-email')

        if (!window.confirm(" voulez-vous supprimer ce privilege pour " + userEmail + " ?"))
            return

        const delUserId = $(element).attr('data-user-id')
        const path = document.getElementById('path').value
        const docType = document.getElementById('doc-type').value;
        const privilegeStrategy = Array.from(document.getElementsByName('rb-privilege-strategy')).filter(r => r.checked)[0].value
        let url = ''
        if (docType === 'document')
            url = '/DocumentPrivilege/Delete'
        else
            url = '/FolderPrivilege/Delete'

        $.ajax({
            url: url,
            method: 'POST',
            data: {
                delUserId,
                path,
                privilegeStrategy
            },
            success: function (resp) {
                document.getElementById('privilegeList').innerHTML = resp
            },
            error: function (error) {
                console.log("error")
            }
        })
    }
})