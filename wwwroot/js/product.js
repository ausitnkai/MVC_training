// 這個檔案使用了第三方程式來調整我們的表格，同時使用了ajax的方式將資料匯入表格中
var dataTable;

$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        // 這邊的 url 要與自己設定的檔案名稱與函式名稱相符
        "ajax": { url: '/admin/products/getall' },
        "columns": [
            { data: 'title', "width": "20%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "20%" },
            { data: 'myProperty.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/products/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> 編輯</a>
                        <a onClick=Delete('/admin/products/delete?id=${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> 刪除</a>
                    </div>`
                },
                "width": "25%"
            }
        ]
    });
}
function Delete(url) {
    Swal.fire({
        title: "你確定要刪除該項目?",
        text: "執行動作後無法復原!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}