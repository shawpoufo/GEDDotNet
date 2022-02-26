
$('#privilegeList').on('click', function (e) {
    const element = e.target
    if (element.classList.contains('btn-modifie-privilege')) {
        const read = $(element).attr('data-read')
        const write = $(element).attr('data-write')
        const download = $(element).attr('data-download')
        const email = $(element).attr('data-email')
        const userId = $(element).attr('data-user-id')
        document.getElementById('read').checked = read == 'True' ? true : false;
        document.getElementById('write').checked = write == 'True' ? true : false;
        document.getElementById('download').checked = download == 'True' ? true : false;
        document.getElementById('newUserId').value = userId
        let dataOption = {
            id: userId,
            text: email
        };
        const newOption = new Option(dataOption.text, dataOption.id, true, true);
        $('#select-search').append(newOption).trigger('change');
        $('#form-state').val('update');
        $('#btnPrivilegeAdd').html('modifier')
        $("#btn-cancel-update").css("display","inline")
    }
    

    
})
