var dataTable;

$(document).ready(function () {
    console.log("here");
    loadDataTable();

})

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: '/admin/User/getall', data: 'data', dataSrc: 'userList' },
        "columns": [
            { data: 'name', "width": "12%" },
            { data: 'userName' },
            { data: 'phoneNumber' },
            { data: 'email', "width": "10%" },
            { data: 'role' },
            { data: 'company.name' },
            {
                data: { id : 'id', lockoutEnd:'lockoutEnd'},
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `<div class="w-75 btn-group" role ="group">
                                <a onclick =Lock('${ data.id }') class="btn btn-primary text-center mx-2"><i class="bi bi-unlock-fill"></i> Unlock User</a>
                                 <a href="/admin/User/UserPermission?${data.id}" class="btn btn-primary text-center mx-2"><i class="bi bi-key-fill"></i> Permissions</a>
                            </div>`
                    }
                    else {
                        return `<div class="w-75 btn-group" role ="group">
                                <a onclick =Lock('${ data.id }') class="btn btn-danger text-center mx-2"><i class="bi bi-lock-fill"></i>Lock User</a>
                                <a href="/admin/User/UserPermission?id=${data.id}" class="btn btn-primary text-center mx-2"><i class="bi bi-key-fill"></i> Permissions</a>
                            </div>`
                    }
                   
                },
                "width": "15%"
            }
        ]
    });

}


function Alert(data) {
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

function Lock(id) {
    $.ajax({
        type: "POST",
        url: '/admin/User/Lock',
        data: JSON.stringify(id),
        contentType: "application/json",
        //success: function (data) {
        //    if (data.succes) {
        //        toastr.succes(data.message);
        //        dataTable.ajax.reload();
        //    }
        //}
    }).then(function (data){
        // Success case
        if (data.success) {
            toastr.success(data.message); // Display success toast
            dataTable.ajax.reload(); // Reload the datatable
        } else {
            // Failure case
            toastr.error(data.message); // Display error toast
            dataTable.ajax.reload(); // Reload the datatable even on failure
        }
    }).catch(function (error) {
        // Error handling in case of AJAX request failure
        console.error('Error occurred:', error);
        toastr.error('An error occurred while processing your request.'); // Display error toast
        dataTable.ajax.reload(); // Reload the datatable on failure
    });
}