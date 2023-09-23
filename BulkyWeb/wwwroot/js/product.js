var dataTable;

$(document).ready(function () {
    console.log("here");
    loadDataTable();
    
})

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: '/admin/product/getall', data:'data', dataSrc: 'productList' },
        "columns": [
            { data: 'title', "width": "12%" },
            { data: 'description' },
            { data: 'isbn' },
            { data: 'author', "width" : "10%"},
            { data: 'listPrice' },
            { data: 'category.name' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role ="group">
                                <a href = "/admin/product/upsert?id=${data}" class="btn btn-primary text-center mx-2"><i class="bi bi-pencil-square " ></i>Edit</a>
                                <a  onClick = Alert("/admin/product/deleteProduct?id=${data}")   class="btn btn-danger text-center mx-2""><i class="bi bi-trash3 "></i>Delete</a>
                            </div>`
                },
                "width": "15%"
            }
        ]
    });
    
}


function  Alert(data) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: data,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.succes(data.message);
                }
            })
        }
    })
}

