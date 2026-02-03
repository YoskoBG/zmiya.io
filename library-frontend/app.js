const apiBase = "https://localhost:5001/api";
const apiBaseLabel = document.getElementById("api-base");
apiBaseLabel.textContent = apiBase;

const booksList = document.getElementById("books-list");
const membersList = document.getElementById("members-list");
const loansList = document.getElementById("loans-list");
const reservationsList = document.getElementById("reservations-list");
const paymentsList = document.getElementById("payments-list");

async function fetchJson(url, options) {
  const response = await fetch(url, options);
  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || "Request failed");
  }
  return response.status === 204 ? null : response.json();
}

function renderList(listElement, items, formatter) {
  listElement.innerHTML = "";
  items.forEach((item) => {
    const li = document.createElement("li");
    li.textContent = formatter(item);
    listElement.appendChild(li);
  });
}

async function loadBooks() {
  const books = await fetchJson(`${apiBase}/books`);
  renderList(booksList, books, (book) => `${book.id}: ${book.title} (${book.publishYear})`);
}

async function loadMembers() {
  const members = await fetchJson(`${apiBase}/members`);
  renderList(membersList, members, (member) => `${member.id}: ${member.fullName}`);
}

async function loadLoans() {
  const loans = await fetchJson(`${apiBase}/loans`);
  renderList(loansList, loans, (loan) => `Loan #${loan.id} - copy ${loan.bookCopyId} for member ${loan.memberId}`);
}

async function loadReservations() {
  const reservations = await fetchJson(`${apiBase}/reservations`);
  renderList(reservationsList, reservations, (reservation) => `Reservation #${reservation.id} - book ${reservation.bookId}`);
}

async function loadPayments() {
  const payments = await fetchJson(`${apiBase}/payments`);
  renderList(paymentsList, payments, (payment) => `Payment #${payment.id} - ${payment.amount} (${payment.method})`);
}

async function refreshAll() {
  await Promise.all([loadBooks(), loadMembers(), loadLoans(), loadReservations(), loadPayments()]);
}

document.getElementById("book-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const form = event.target;
  const payload = {
    title: form.title.value,
    isbn: form.isbn.value,
    publishYear: Number(form.publishYear.value),
    publisherId: Number(form.publisherId.value),
  };
  await fetchJson(`${apiBase}/books`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  form.reset();
  await loadBooks();
});

document.getElementById("member-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const form = event.target;
  const payload = {
    fullName: form.fullName.value,
    email: form.email.value,
    phone: form.phone.value,
    joinDate: new Date().toISOString(),
  };
  await fetchJson(`${apiBase}/members`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  form.reset();
  await loadMembers();
});

document.getElementById("loan-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const form = event.target;
  const payload = {
    memberId: Number(form.memberId.value),
    bookCopyId: Number(form.bookCopyId.value),
  };
  await fetchJson(`${apiBase}/loans`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  form.reset();
  await loadLoans();
});

document.getElementById("reservation-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const form = event.target;
  const payload = {
    memberId: Number(form.memberId.value),
    bookId: Number(form.bookId.value),
  };
  await fetchJson(`${apiBase}/reservations`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  form.reset();
  await loadReservations();
});

document.getElementById("payment-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const form = event.target;
  const payload = {
    loanId: Number(form.loanId.value),
    amount: Number(form.amount.value),
    method: form.method.value,
  };
  await fetchJson(`${apiBase}/payments`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  form.reset();
  await loadPayments();
});

refreshAll().catch((error) => {
  console.error(error);
});
