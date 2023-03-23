
// Get references to the input fields
const field1 = document.getElementById('fuelBalIn');
const field2 = document.getElementById('fuelFillUp');
const field3 = document.getElementById('fuelConsumNorm');

// Add an event listener to update field3 whenever field1 or field2 changes
field1.addEventListener('input', updateField3);
field2.addEventListener('input', updateField3);

// Function to update field3
function updateField3() {
	const sum = parseInt(field1.value) + parseInt(field2.value);
	field3.value = sum;
}