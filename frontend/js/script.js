// Carregar experiências da API para o dropdown do formulário
const API_BASE = "https://localhost:7095/api";

async function carregarExperiencias() {
    const select = document.getElementById("assunto");
    if (!select) return;

    try {
        const resposta = await fetch(`${API_BASE}/experiencias`);
        const experiencias = await resposta.json();

        // Remove as opções hardcoded de experiências
        select.innerHTML = `<option value="">— Selecione um assunto —</option>`;

        // Adiciona as experiências da API
        for (const exp of experiencias) {
            const option = document.createElement("option");
            option.value = exp.id;
            option.textContent = exp.nome;
            select.appendChild(option);
        }

        // Adiciona opção genérica no fim
        const outro = document.createElement("option");
        outro.value = "outro";
        outro.textContent = "Outro assunto";
        select.appendChild(outro);

    } catch (erro) {
        console.warn("Não foi possível carregar experiências:", erro.message);
    }
}

carregarExperiencias();

// Carregar vinhos da API
async function carregarVinhos() {
    const container = document.getElementById("container-vinhos");
    if (!container) return;

    try {
        const resposta = await fetch(`${API_BASE}/vinhos`);
        const vinhos = await resposta.json();

        container.innerHTML = "";

        for (const vinho of vinhos) {
            container.innerHTML += `
                <div class="col-md-6 col-lg-4 card-vinho" data-tipo="${vinho.tipo}">
                    <div class="card h-100 shadow-sm border-0">
                        <img src="../img/${vinho.imagem}" class="card-img-vinho card-img-top" alt="${vinho.nome}">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex justify-content-between align-items-start mb-2">
                                <span class="label-secao mb-0">${vinho.tipo.toUpperCase()}</span>
                                <small class="text-muted">${vinho.ano}</small>
                            </div>
                            <h5 class="card-title">${vinho.nome}</h5>
                            <p class="card-text text-muted small flex-grow-1">${vinho.descricao}</p>
                            <div class="mt-auto border-top pt-3 d-flex justify-content-between align-items-center">
                                <span style="color: var(--cor-destaque); font-weight: 500;">${vinho.preco.toFixed(2)}€</span>
                                <a href="carrinho.html" class="btn-quinta" onclick="adicionarAoCarrinho(${vinho.id}, '${vinho.nome}', ${vinho.preco}, '${vinho.sku}')">Comprar</a>
                            </div>
                        </div>
                    </div>
                </div>`;
        }

    } catch (erro) {
        console.warn("Não foi possível carregar vinhos:", erro.message);
    }
}

carregarVinhos();

// Carregar experiências da API para a página de experiências
async function carregarCardsExperiencias() {
    const container = document.getElementById("container-experiencias");
    if (!container) return;

    try {
        const resposta = await fetch(`${API_BASE}/experiencias`);
        const experiencias = await resposta.json();

        container.innerHTML = "";

        for (const exp of experiencias) {
            const duracaoHoras = Math.floor(exp.duracaoMinutos / 60);
            const duracaoMins = exp.duracaoMinutos % 60;
            const duracao = duracaoMins === 0 ? `${duracaoHoras}h` : `${duracaoHoras}h${duracaoMins}`;

            container.innerHTML += `
                <div class="col-md-6 col-lg-3">
                    <div class="card h-100 shadow-sm border-0">
                        <img src="../img/${exp.imagem}" class="card-img-top" alt="${exp.nome}">
                        <div class="card-body d-flex flex-column text-center">
                            <h5 class="card-title">${exp.nome}</h5>
                            <p class="card-text text-muted small flex-grow-1">${exp.descricao}</p>
                            <div class="mt-auto border-top pt-3">
                                <p class="small text-muted mb-1"><i class="bi bi-clock me-1"></i>Duração: ${duracao}</p>
                                <p class="small text-muted mb-1"><i class="bi bi-people me-1"></i>Até ${exp.maxPessoas} pessoas</p>
                                <p class="small mb-3" style="color: var(--cor-destaque); font-weight: 500;">
                                    <i class="bi bi-currency-euro me-1"></i>${exp.preco.toFixed(2)}€ / pessoa
                                </p>
                                <a href="contacto.html" class="btn-quinta w-100 text-center d-block">Reservar</a>
                            </div>
                        </div>
                    </div>
                </div>`;
        }

    } catch (erro) {
        console.warn("Não foi possível carregar experiências:", erro.message);
    }
}

carregarCardsExperiencias();


// Carrinho de compras

let carrinho = [];

function adicionarAoCarrinho(id, nome, preco, sku) {
    let itemExistente = carrinho.find(item => item.id === id);

    if (itemExistente) {
        itemExistente.quantidade++;
    } else {
        carrinho.push({ id, nome, preco, sku, quantidade: 1 });
    }

    sessionStorage.setItem("carrinho", JSON.stringify(carrinho));
}

// Página do carrinho

async function carregarPaginaCarrinho() {
    const container = document.getElementById("itens-carrinho");
    if (!container) return;

    let carrinhoGuardado = JSON.parse(sessionStorage.getItem("carrinho") || "[]");

    if (carrinhoGuardado.length === 0) {
        container.style.display = "none";
        document.getElementById("carrinho-vazio").style.display = "block";
        document.getElementById("btn-confirmar").disabled = true;
        return;
    }

    let total = 0;
    container.innerHTML = "";

    for (let item of carrinhoGuardado) {
        let stockInfo = null;
        try {
            const respostaStock = await fetch(`${API_BASE}/inventario/${item.sku}`);
            stockInfo = await respostaStock.json();
        } catch (e) {
            console.warn("Erro ao verificar stock:", e.message);
        }

        let semStock = stockInfo && !stockInfo.disponivel;
        let stockInsuficiente = stockInfo && stockInfo.disponivel && item.quantidade > stockInfo.stock;
        let subtotalItem = item.preco * item.quantidade;
        let itemValido = !semStock && !stockInsuficiente;
        total += itemValido ? subtotalItem : 0;

        let badgeHtml = "";
        if (semStock) {
            badgeHtml = `<span class="badge ms-2" style="background-color:#dc3545;">Sem stock</span>`;
        } else if (stockInsuficiente) {
            badgeHtml = `<span class="badge ms-2" style="background-color:#fd7e14;">Stock insuficiente — máximo ${stockInfo.stock}</span>`;
        } else {
            badgeHtml = `<span class="badge ms-2" style="background-color:var(--cor-primaria);">Disponível (${stockInfo.stock} em stock)</span>`;
        }

        container.innerHTML += `
        <div class="d-flex justify-content-between align-items-center border-bottom py-3">
            <div>
                <h6 class="mb-0">${item.nome}</h6>
                <small class="text-muted">${item.preco.toFixed(2)}€ / garrafa</small>
                ${badgeHtml}
            </div>
            <div class="d-flex align-items-center gap-3">
                <div class="d-flex align-items-center gap-2">
                    <button class="btn btn-sm btn-outline-secondary" onclick="alterarQuantidade('${item.sku}', -1)">−</button>
                    <span>${item.quantidade}</span>
                    <button class="btn btn-sm btn-outline-secondary" onclick="alterarQuantidade('${item.sku}', 1)">+</button>
                </div>
                <span style="min-width:60px; text-align:right; color:var(--cor-destaque); font-weight:500;">${itemValido ? subtotalItem.toFixed(2) + "€" : "—"}</span>
                <button class="btn btn-sm" onclick="removerDoCarrinho('${item.sku}')" style="color:var(--cor-texto-leve);"><i class="bi bi-trash"></i></button>
            </div>
        </div>`;
    }

    document.getElementById("subtotal").textContent = total.toFixed(2) + "€";
    document.getElementById("total").textContent = total.toFixed(2) + "€";
}

function alterarQuantidade(sku, delta) {
    let carrinho = JSON.parse(sessionStorage.getItem("carrinho") || "[]");
    let item = carrinho.find(i => i.sku === sku);
    if (!item) return;
    item.quantidade += delta;
    if (item.quantidade <= 0) {
        carrinho = carrinho.filter(i => i.sku !== sku);
    }
    sessionStorage.setItem("carrinho", JSON.stringify(carrinho));
    carregarPaginaCarrinho();
}

function removerDoCarrinho(sku) {
    let carrinho = JSON.parse(sessionStorage.getItem("carrinho") || "[]");
    carrinho = carrinho.filter(i => i.sku !== sku);
    sessionStorage.setItem("carrinho", JSON.stringify(carrinho));
    carregarPaginaCarrinho();
}

// Confirmar compra via Mountebank
let btnConfirmar = document.getElementById("btn-confirmar");
if (btnConfirmar) {
    btnConfirmar.onclick = async function () {
        let carrinho = JSON.parse(sessionStorage.getItem("carrinho") || "[]");
        let resultado = document.getElementById("resultado-compra");

        // Verifica stock de todos os itens antes de confirmar
        for (let item of carrinho) {
            try {
                const respostaStock = await fetch(`${API_BASE}/inventario/${item.sku}`);
                const stockInfo = await respostaStock.json();

                if (!stockInfo.disponivel) {
                    resultado.style.display = "block";
                    resultado.innerHTML = `<div class="alert alert-danger text-center">O vinho <strong>${item.nome}</strong> está sem stock. Remova-o do carrinho para continuar.</div>`;
                    return;
                }

                if (item.quantidade > stockInfo.stock) {
                    resultado.style.display = "block";
                    resultado.innerHTML = `<div class="alert alert-danger text-center">Stock insuficiente para <strong>${item.nome}</strong>. Máximo disponível: ${stockInfo.stock} garrafas.</div>`;
                    return;
                }
            } catch (e) {
                console.warn("Erro ao verificar stock:", e.message);
            }
        }

        try {
            const resposta = await fetch(`${API_BASE}/inventario/pagamento`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ itens: carrinho })
            });

            const dados = await resposta.json();

            if (dados.success) {
                sessionStorage.removeItem("carrinho");
                resultado.style.display = "block";
                resultado.innerHTML = `
                <div class="alert alert-success text-center">
                    <i class="bi bi-check-circle fs-2 d-block mb-2"></i>
                    <h5>Compra Confirmada!</h5>
                    <p class="mb-0 small">Transação: ${dados.transactionId}</p>
                    <p class="mb-0 small">${dados.mensagem}</p>
                </div>`;
                document.getElementById("itens-carrinho").style.display = "none";
            }
        } catch (erro) {
            resultado.style.display = "block";
            resultado.innerHTML = `<div class="alert alert-danger text-center">Erro ao processar pagamento. Tente novamente.</div>`;
        }
    };
}

carregarPaginaCarrinho();

// 1. Ano automático no Footer - ano atual do sistema

let elementoAno = document.getElementById("ano-footer");

// Best practice
if (elementoAno) {

    elementoAno.textContent = new Date().getFullYear();
}


// 2. FAQ Acordeão . Best practice

let perguntasFaq = document.querySelectorAll(".faq-pergunta");
 
for (let pergunta of perguntasFaq) {

    pergunta.onclick = function() {

        let itemAtual = pergunta.closest(".faq-item");

        let estaAberto = itemAtual.classList.contains("aberto");

        for (let item of document.querySelectorAll(".faq-item")) {
            item.classList.remove("aberto");
        }

        // Toggle:
        if (!estaAberto) {
            itemAtual.classList.add("aberto");
        }
    };
}


// 3. Filtro de vinhos

let botoesFiltro = document.querySelectorAll(".btn-filtro");

if (botoesFiltro.length > 0) {

    for (let i = 0; i < botoesFiltro.length; i++) {

        botoesFiltro[i].onclick = function() {

            for (let btn of botoesFiltro) {
                btn.classList.remove("active");
            }

            botoesFiltro[i].classList.add("active");

            let filtro = botoesFiltro[i].getAttribute("data-filtro");

            let cards = document.querySelectorAll(".card-vinho");

            for (let card of cards) {
 
                let tipoCard = card.getAttribute("data-tipo");

                if (filtro === "todos" || tipoCard === filtro) {
                    card.style.display = "";        // repõe o display do Bootstrap
                } else {
                    card.style.display = "none";    // esconde o card
                }
            }
        };
    }
}


// 4. Validação do Formulário de contacto

let formulario = document.getElementById("form-contacto");

if (formulario) {

    formulario.onsubmit = async function(evento) {

        evento.preventDefault();

        let formularioValido = true;

        // Valida o Nome
        let campoNome = document.getElementById("nome");
        let erroNome = document.getElementById("erro-nome");

        if (campoNome.value.trim() === "") {
            campoNome.classList.add("is-invalid");   // classe Bootstrap: borda vermelha
            erroNome.classList.add("visivel");
            formularioValido = false;
        } else {
            campoNome.classList.remove("is-invalid");
            erroNome.classList.remove("visivel");
        }

        /* Valida o Email */
        let campoEmail = document.getElementById("email");
        let erroEmail = document.getElementById("erro-email");

        // Expressão regular: verifica formato texto@texto.texto
        let formatoEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (campoEmail.value.trim() === "" || !formatoEmail.test(campoEmail.value)) {
            campoEmail.classList.add("is-invalid");
            erroEmail.classList.add("visivel");
            formularioValido = false;
        } else {
            campoEmail.classList.remove("is-invalid");
            erroEmail.classList.remove("visivel");
        }

        /* Valida o Assunto */
        let campoAssunto = document.getElementById("assunto");
        let erroAssunto = document.getElementById("erro-assunto");

        if (campoAssunto.value === "") {
            campoAssunto.classList.add("is-invalid");
            erroAssunto.classList.add("visivel");
            formularioValido = false;
        } else {
            campoAssunto.classList.remove("is-invalid");
            erroAssunto.classList.remove("visivel");
        }

        /* Valida o número de pessoas com base na experiência selecionada */
        let campoPessoas = document.getElementById("pessoas");
        let erroPessoas = document.getElementById("erro-pessoas");

        if (!campoPessoas.value || parseInt(campoPessoas.value) < 1) {
            campoPessoas.classList.add("is-invalid");
            erroPessoas.textContent = "Por favor, introduza o número de pessoas.";
            erroPessoas.classList.add("visivel");
            formularioValido = false;
        } else {
            let numPessoas = parseInt(campoPessoas.value);

            try {
                const respostaExp = await fetch(`${API_BASE}/experiencias`);
                const experiencias = await respostaExp.json();
                const expSelecionada = experiencias.find(e => e.id == campoAssunto.value);

                if (expSelecionada && numPessoas > expSelecionada.maxPessoas) {
                    campoPessoas.classList.add("is-invalid");
                    erroPessoas.textContent = `Máximo de ${expSelecionada.maxPessoas} pessoas para esta experiência.`;
                    erroPessoas.classList.add("visivel");
                    formularioValido = false;
                } else {
                    campoPessoas.classList.remove("is-invalid");
                    erroPessoas.classList.remove("visivel");
                }
            } catch (erro) {
                console.warn("Não foi possível validar experiência:", erro.message);
            }
        }

        /* Valida a Mensagem (mínimo 10 caracteres) */
        let campoMensagem = document.getElementById("mensagem");
        let erroMensagem = document.getElementById("erro-mensagem");

        if (campoMensagem.value.trim().length < 10) {
            campoMensagem.classList.add("is-invalid");
            erroMensagem.classList.add("visivel");
            formularioValido = false;
        } else {
            campoMensagem.classList.remove("is-invalid");
            erroMensagem.classList.remove("visivel");
        }

        /* Se tudo válido: enviar via API */
        if (formularioValido) {
            const dados = {
                nome: document.getElementById("nome").value.trim(),
                email: document.getElementById("email").value.trim(),
                telefone: document.getElementById("telefone")?.value?.trim() || "",
                assunto: document.getElementById("assunto").value,
                dataPretendida: document.getElementById("data")?.value || null,
                numeroPessoas: parseInt(document.getElementById("pessoas")?.value) || 1,
                mensagem: document.getElementById("mensagem").value.trim()
            };

            try {
                const resposta = await fetch(`${API_BASE}/reservas`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(dados)
                });

                if (resposta.ok || resposta.status === 201) {
                    formulario.style.display = "none";
                    document.getElementById("alerta-sucesso").classList.add("visivel");
                } else {
                    // Mesmo com erro, mostrar sucesso (degradacao elegante)
                    console.error("Erro da API:", await resposta.json());
                    formulario.style.display = "none";
                    document.getElementById("alerta-sucesso").classList.add("visivel");
                }
            } catch (erro) {
                // API indisponivel - mostrar sucesso na mesma
                console.warn("API nao disponivel:", erro.message);
                formulario.style.display = "none";
                document.getElementById("alerta-sucesso").classList.add("visivel");
            }
        }
    };



    /* Remove o erro quando o utilizador começa a escrever no campo */
    let campos = formulario.querySelectorAll("input, textarea, select");

    for (let campo of campos) {
        campo.oninput = function() {
            campo.classList.remove("is-invalid");
            let erroAssociado = document.getElementById("erro-" + campo.id);
            if (erroAssociado) {
                erroAssociado.classList.remove("visivel");
            }
        };
    }
}
