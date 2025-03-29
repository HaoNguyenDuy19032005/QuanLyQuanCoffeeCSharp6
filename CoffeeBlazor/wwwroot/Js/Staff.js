let cart = [];
let totalAmount = 0;
let discountPercentage = 0;

document.addEventListener('DOMContentLoaded', () => {
    initializeFilters();
    checkCartStatus();
});

function initializeFilters() {
    document.querySelectorAll('.filter-button button').forEach(button => {
        button.addEventListener('click', function () {
            document.querySelector('.filter-button button.active')?.classList.remove('active');
            this.classList.add('active');
            let categoryId = this.getAttribute('data-category');
            filterProducts(categoryId === 'all' ? 'Tất cả' : categoryId);
        });
    });
    document.getElementById('discount').addEventListener('change', function () {
        discountPercentage = parseInt(this.value);
        updateCart();
    });
}

function filterProducts(categoryId) {
    console.log(`Filtering by category: ${categoryId}`);
    const productsSection = document.getElementById('productsSection');
    const products = document.querySelectorAll('.product');
    let hasVisibleProducts = false;

    products.forEach(product => {
        const productCategoryId = product.getAttribute('data-category');
        console.log(`Product: ${product.querySelector('.product-name').textContent}, Category: ${productCategoryId}`);
        if (categoryId === "Tất cả" || productCategoryId === categoryId) {
            product.style.display = '';
            product.style.height = 'auto';
            hasVisibleProducts = true;
        } else {
            product.style.display = 'none';
        }
    });

    productsSection.style.display = 'grid';
    productsSection.style.gap = '13px';
    productsSection.style.maxWidth = '100%';
    productsSection.style.width = '100%';

    const noProductsMessage = productsSection.querySelector('.no-products');
    if (!hasVisibleProducts && !noProductsMessage) {
        const message = document.createElement('p');
        message.textContent = 'Không có sản phẩm nào trong danh mục này.';
        message.className = 'no-products';
        productsSection.appendChild(message);
    } else if (hasVisibleProducts && noProductsMessage) {
        noProductsMessage.remove();
    }
}

function searchProducts() {
    const searchTerm = document.getElementById('search').value.trim().toLowerCase();
    const products = document.querySelectorAll('.product');
    let hasVisibleProducts = false;
    const productsSection = document.getElementById('productsSection');

    if (searchTerm === '') {
        products.forEach(product => {
            product.style.display = '';
            product.style.height = 'auto';
        });
        productsSection.style.gap = '13px';
        return;
    }

    productsSection.style.display = 'grid';
    productsSection.style.gap = '13px';
    productsSection.style.maxWidth = '100%';
    productsSection.style.width = '100%';

    products.forEach(product => {
        const productName = product.getAttribute('data-name').toLowerCase().trim();

        if (normalizeString(productName).includes(normalizeString(searchTerm))) {
            product.style.display = '';
            product.style.height = 'auto';

            hasVisibleProducts = true;
        } else {
            product.style.display = 'none';
        }
    });

    const noProductsMessage = productsSection.querySelector('.no-products');
    if (!hasVisibleProducts && searchTerm !== '') {
        if (!noProductsMessage) {
            const message = document.createElement('p');
            message.textContent = 'Không tìm thấy sản phẩm nào.';
            message.className = 'no-products';
            productsSection.appendChild(message);
        }
    } else if (hasVisibleProducts && noProductsMessage) {
        noProductsMessage.remove();
    }

    console.log(`Search term: "${searchTerm}", Visible products: ${hasVisibleProducts}`);
}

// Hàm chuẩn hóa để loại bỏ dấu và chuyển sang chữ thường
function normalizeString(str) {
    return str
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .toLowerCase();
}


function addToCart(productName, productPrice, productImg) {
    let existingProduct = cart.find(product => product.name === productName);

    if (existingProduct) {
        existingProduct.quantity++;
    } else {

        cart.push({ name: productName, price: productPrice, img: productImg, quantity: 1 });
    }

    updateCart();
    checkCartStatus();
}
function checkCartStatus() {
    const isCartEmpty = cart.length === 0;
    const elements = [
        'confirm-btn',
        'export-btn',
        'discount',
        'customer-money',
        'note',
        'customer-id'
    ];
    elements.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.disabled = isCartEmpty;
        } else {
            console.error(`Không tìm thấy phần tử với ID: ${id}`);
        }
    });
} function updateCart() {
    console.log('Updating cart:', cart);
    let orderTableBody = document.querySelector('#orderTableBody');
    orderTableBody.innerHTML = '';
    totalAmount = 0;

    cart.forEach(product => {
        let row = document.createElement('tr');
        row.innerHTML = `
                        <td>${product.name}</td>
                        <td>${formatNumber(product.price)} VND</td>
                        <td>
                            <button class="quantity-btn" onclick="updateQuantity('${product.name}', -1)">-</button>
                            ${product.quantity}
                            <button class="quantity-btn" onclick="updateQuantity('${product.name}', 1)">+</button>
                        </td>
                        <td><button class="remove-btn" onclick="removeProduct('${product.name}')">X</button></td>
                    `;
        orderTableBody.appendChild(row);
        totalAmount += product.price * product.quantity;
    });

    let discountedAmount = totalAmount - (totalAmount * discountPercentage / 100);
    document.getElementById('total-amount').textContent = formatNumber(discountedAmount) + " đ";
    calculateChange();

    checkCartStatus();
}
function updateCart() {
    console.log('Updating cart:', cart);
    let orderTableBody = document.querySelector('#orderTableBody');
    orderTableBody.innerHTML = '';
    totalAmount = 0;

    cart.forEach(product => {
        let row = document.createElement('tr');
        row.innerHTML = `
            <td>${product.name}</td>
            <td>${formatNumber(product.price)} VND</td>
            <td>
                <button class="quantity-btn" onclick="updateQuantity('${product.name}', -1)">-</button>
                ${product.quantity}
                <button class="quantity-btn" onclick="updateQuantity('${product.name}', 1)">+</button>
            </td>
            <td><button class="remove-btn" onclick="removeProduct('${product.name}')">X</button></td>
        `;
        orderTableBody.appendChild(row);
        totalAmount += product.price * product.quantity;
    });

    let discountedAmount = totalAmount - (totalAmount * discountPercentage / 100);
    document.getElementById('total-amount').textContent = formatNumber(discountedAmount) + " đ";
    calculateChange();

    checkCartStatus();
}

function updateQuantity(productName, delta) {
    let product = cart.find(p => p.name === productName);
    if (product) {
        product.quantity += delta;
        if (product.quantity <= 0) {
            removeProduct(productName);
        } else {
            updateCart();
        }
    }
}

function removeProduct(productName) {
    cart = cart.filter(product => product.name !== productName);
    updateCart();
}

function calculateChange() {
    let customerMoney = parseFloat(document.getElementById('customer-money').value.replace(/\./g, '').replace(',', '')) || 0;
    let changeElement = document.getElementById('change');
    let errorMessageElement = document.getElementById('error-message');
    let confirmButton = document.getElementById('confirm-btn');

    if (changeElement && errorMessageElement) {
        if (isNaN(customerMoney) || customerMoney === 0) {
            changeElement.value = '';
            errorMessageElement.style.display = 'none';
            confirmButton.disabled = true;
        } else {
            // Tính số tiền đã giảm giá
            let discountedAmount = totalAmount - (totalAmount * discountPercentage / 100);

            if (customerMoney < discountedAmount) {
                changeElement.value = '';
                errorMessageElement.style.display = 'block';
                confirmButton.disabled = true;
            } else {
                let change = customerMoney - discountedAmount;
                changeElement.value = formatNumber(change) + " đ";
                errorMessageElement.style.display = 'none';
                confirmButton.disabled = false;
            }
        }
    } else {
        console.error("Lỗi: Không tìm thấy phần tử changeElement hoặc errorMessageElement trong DOM.");
    }
}

function handlePayment() {
    const customerMoney = parseFloat(document.getElementById('customer-money').value.replace(/\./g, '')) || 0;
    const discountedAmount = totalAmount * (1 - discountPercentage / 100);

    if (customerMoney === 0) {
        alert('Vui lòng nhập số tiền khách đưa.');
    } else if (customerMoney < discountedAmount) {
        alert('Số tiền không đủ để thanh toán.');
    } else {
        alert('Thanh toán thành công!');
        resetOrder();
    }
}

function resetOrder() {
    cart = [];
    updateCart();
    document.getElementById('customer-money').value = '';
    document.getElementById('change').value = '';
    document.getElementById('discount').value = '0';
    document.getElementById('total-amount').textContent = '0 đ';
    document.getElementById('error-message').style.display = 'none';
    document.getElementById('note').value = '';
    document.getElementById('customer-id').value = '';
}

function toggleDropdown() {
    const dropdown = document.getElementById('dropdownMenu');
    dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
}


function formatMoney(input) {
    let value = input.value.replace(/\D/g, '');
    if (value.length > 0) {
        input.value = value.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    }
    calculateChange();
}

function formatNumber(number) {
    return number.toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

function exportInvoice() {
    if (cart.length === 0) {
        alert('Không có đơn hàng để xuất hóa đơn.');
        return;
    }

    const customerMoney = parseFloat(document.getElementById('customer-money').value.replace(/\./g, '').replace(',', '')) || 0;
    if (isNaN(customerMoney) || customerMoney === 0) {
        alert('Vui lòng nhập số tiền khách đưa.');
        return;
    }

    generatePDF();
}

function generatePDF() {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({
        orientation: 'portrait',
        unit: 'mm',
        format: 'a4'
    });

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(12);

    const primaryColor = '#006F3C';
    const textColor = '#333';
    const backgroundColor = '#f4f4f4';

    doc.setFontSize(20);
    doc.setTextColor(primaryColor);
    doc.setFont('helvetica', 'bold');
    doc.text('HÓA ĐƠN', 105, 20, { align: 'center' });

    doc.setFontSize(10);
    doc.setTextColor(textColor);
    doc.setFont('helvetica', 'normal');
    doc.text('Cửa hàng: Lion Store', 20, 40);
    doc.text('Địa chỉ: Hà Nội, Việt Nam', 20, 50);
    doc.text('SĐT: 0987654321', 20, 60);

    doc.setFontSize(12);
    doc.text(`Khách hàng: Nguyễn Quốc Anh`, 20, 80);
    doc.text(`Ngày xuất: ${new Date().toLocaleString()}`, 20, 90);

    let yPosition = 110;
    doc.setFontSize(12);
    doc.setTextColor(textColor);
    doc.setFont('helvetica', 'normal');

    doc.setFont('helvetica', 'bold');
    doc.text('Sản phẩm', 20, yPosition);
    doc.text('Giá', 100, yPosition, { align: 'right' });
    doc.text('SL', 130, yPosition, { align: 'center' });
    doc.text('TT', 160, yPosition, { align: 'right' });
    yPosition += 5;

    doc.setDrawColor(200, 200, 200);
    doc.setLineWidth(0.2);
    doc.line(15, yPosition, 195, yPosition);
    yPosition += 5;

    cart.forEach(product => {
        const productTotal = product.price * product.quantity;
        doc.text(product.name, 20, yPosition);
        doc.text(formatNumber(product.price) + " VND", 100, yPosition, { align: 'right' });
        doc.text(product.quantity.toString(), 130, yPosition, { align: 'center' });
        doc.text(formatNumber(productTotal) + " VND", 160, yPosition, { align: 'right' });
        yPosition += 10;

        doc.line(15, yPosition, 195, yPosition);
    });

    doc.setFont('helvetica', 'bold');
    const totalAmountWithDiscount = totalAmount * (1 - discountPercentage / 100);
    doc.text(`Tổng cộng: ${formatNumber(totalAmountWithDiscount)} VND`, 160, yPosition + 5, { align: 'right' });
    yPosition += 10;

    const customerMoney = parseFloat(document.getElementById('customer-money').value.replace(/\./g, '').replace(',', '')) || 0;
    const change = customerMoney - totalAmountWithDiscount;
    doc.text(`Khách đưa: ${formatNumber(customerMoney)} VND`, 160, yPosition + 5, { align: 'right' });
    yPosition += 10;
    doc.text(`Tiền thừa: ${formatNumber(change)} VND`, 160, yPosition + 5, { align: 'right' });

    doc.setFontSize(10);
    doc.setTextColor(primaryColor);
    doc.text('Cảm ơn quý khách! Powered by Lion Store', 105, 280, { align: 'center' });
    doc.setTextColor(textColor);
    doc.text('Liên hệ: 0987654321 | www.lion.com', 105, 290, { align: 'center' });

    doc.save('HoaDon_Receipt.pdf');
}


