﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "5%" },
            { "data": "email", "width": "5%" },
            { "data": "phoneNumber", "width": "5%" },
            { "data": "company.name", "width": "5%" },
            { "data": "role", "width": "5%" },
            {
                "data": {
                    id:"id",
                    lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockOut = new Date(data.lockoutEnd).getTime();

                    if (lockOut > today) {
                        //user locked
                        return ` 
                            <div class="text-center">
                                <a  onclick=LockUnlock('${data.id}')  class="btn btn-danger">Unlock</a>
                            </div>
                        `;
                    }
                    else {
                        //user Unlock
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-info" >Lock</a>
                            </div>
                             `;

                    }
                }
            }


        ]
    })
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/User/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message)
                dataTable.ajax.reload();

            }
            else {
                toastr.error(data.message);
            }
        }
    })
}


//var dataTable;

//$(document).ready(function () {
//    loadDataTable();
//})
//function loadDataTable() {
//    dataTable = $('#tblData').DataTable({
//        "ajax": {
//            "url": "/Admin/User/GetAll"
//        },
//        "columns": [
//            { "data": "name", "width": "15%" },
//            { "data": "email", "width": "15%" },
//            { "data": "phoneNumber", "width": "15%" },
//            { "data": "company.name", "width": "15%" },
//            { "data": "role", "width": "15%" },
//            {
//                "data": {
//                    id: "id", lockoutEnd: "lockoutEnd"
//                },
//                "render": function (data) {
//                    var today = new Date().getTime();
//                    var lockout = new Date(data.lockoutEnd).getTime();
//                    if (lockout > today) {
//                        //user locked
//                        return `
//                                <div class="text-center">
//                                    <a onclick=LockUnLock('${data.id}') class="btn btn-danger">
//                                        UnLock
//                                     </a>
//                                </div>
//                            `;
//                    }
//                    else {
//                        //user unlock
//                        return `
//                                <div class="text-center">
//                                    <a onclick=LockUnLock('${data.id}') class="btn btn-success">
//                                        Lock
//                                     </a>
//                                </div>
//                            `;
//                    }
//                }
//            }
//        ]
//    })
//}

//function LockUnLock(id) {
//    $.ajax({
//        type: "POST",
//        url: "/Admin/User/LockUnlock",
//        data: JSON.stringify(id),
//        contentType: "application/json",
//        success: function (data) {
//            if (data.success) {
//                toastr.success(data.message);
//                dataTable.ajax.reload();
//            }
//            else {
//                toastr.error(data.message);
//            }
//        }

//    })
//}