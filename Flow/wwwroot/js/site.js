// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// Open task creation modal
$('#addTaskBtn').click(function () {
    $('#taskModal').modal('show');
});

// Handle task submission
$('#saveTaskBtn').click(function () {
    // Perform task creation form submission via AJAX or other method
    // Upon successful submission, close the modal
    $('#taskModal').modal('hide');
});

$('#hideTaskBtn').click(function () {
    // Perform task creation form submission via AJAX or other method
    // Upon successful submission, close the modal
    $('#taskModal').modal('hide');
});

$('#CloseBtn').click(function () {
    // Perform task creation form submission via AJAX or other method
    // Upon successful submission, close the modal
    $('#taskModal').modal('hide');
});