var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').dataTable({
        "ajax": {
            "url": "/Admin/CoverType/GetAll"
        },
        "columns": [{ "data": "name", "width": "70%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a class="btn btn-info" href="/Admin/CoverType/Upsert/${data}">Edit</a>
                            <a class="btn btn-danger" onclick=Delete("/Admin/CoverType/Delete/${data}")>Delete</a>
                             
                        </div>

                        `;
                }
            }
        ]
    })
}
function Delete(url) {
    swal({
        title: "Want to delete data",
        text: "Delete Information",
        icon: "warning",
        dangerModel: true,
        buttons: true
    }).then((willdelete) => {
        if (willdelete) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}