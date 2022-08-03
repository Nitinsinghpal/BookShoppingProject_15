var dataTable;
$(document).ready(function () {
    LoadDataTable();
})
function LoadDataTable() {
    dataTable = $('#tblData').DataTable({

        //"lengthMenu": [1, 2, 3, 4, 5, 6],
        "paging": false,
        //"searching": false,
        "scrollY": "200px",
        //"scrollX": "600px",
        //"fixedColumns": false,
        //"order": [0, 'asc'],
        //"scrollCollpase": true,
        //paging: false,
        //scrollX: "500px",
        //"scrollCollapse": true,
        //searching: false,
        //ordering: false,
        //"processing": true,
        //"order": [0, 'desc'],

        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%", "className": "text-center" },
            { "data": "description", "width": "15%" },
            {
                "data": null,
                "render": function (data) {
                    return data['author'] + ", " + data['isbn'];
                }
            },
            { "data": "isbn", "width": "15%", "className": "dt-body-right" },
            { "data": "price", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                    <a class="btn btn-info" href="/Admin/Product/upsert/${data}">
                     <i class="fas fa-edit"></i>
                    </a>
                    
                    <a class="btn btn-danger" OnClick=Delete("/Admin/Product/Delete/${data}")>
                    <i class="fas fa-trash-alt"></i>
                    </a>
                    </div>
                `;
                }
            }
        ]
    })
}
function Delete(url) {
    swal({
        title: "Want to Delete Data?",
        text: "Delete Information !!!",
        buttons: true,
        //buttons: {
        //    text:"cbjn",
        //},
        //buttons: ["stop", "Do it"],
        //buttons: {
        //    confirm: true,
        //    cancel: true,

        //},
        timer: 3000,
        className: "red-bg",
        //content: {
        //    element: "input",
        //    attributes: {
        //        placeHolder: "type your name here",
        //        type:"password"
        //    }
        //},
        buttons: {
            confirm: true,  
            cancel: true,
            roll: true
        },
        icon: "warning",
        dangerModel: true
    }).then((WillDelete) => {
        if (WillDelete) {
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