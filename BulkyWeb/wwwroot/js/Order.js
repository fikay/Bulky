var dataTable
$(document).ready(function () {
    console.log("here");
    var query = window.location.search
    /*console.log(query);*/
    const urlParams = new URLSearchParams(query);
    console.log(urlParams);
    loadDataTable(query);

})

function loadDataTable(query) {
    
    var url = `/admin/Order/getall${query}`;
    console.log(url);

    dataTable = $('#myTable').DataTable({
        "ajax": { url: url, data: 'data' },
        "columns": [
            { data: 'id', "width": "12%" },
            { data: 'name' },
            { data: 'applicationUser.phoneNumber' },
            { data: 'applicationUser.email', "width": "10%" },
            { data: 'orderStatus' },
            { data: 'orderTotal' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role ="group">
                                <a href = "/admin/Order/Details?id=${data}" class="btn btn-primary text-center mx-2"><i class="bi bi-pencil-square " ></i></a>
                              </div>`   
                },
                "width": "15%"
            }
        ]
    });

}
