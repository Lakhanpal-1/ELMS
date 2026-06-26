// ELMS — sidebar toggle + active nav highlighting
(function () {
    'use strict';

    const sidebar = document.querySelector('.sidebar-nav');
    const toggle = document.querySelector('.sidebar-toggle');

    // Backdrop for mobile
    let backdrop = document.querySelector('.sidebar-backdrop');
    if (!backdrop) {
        backdrop = document.createElement('div');
        backdrop.className = 'sidebar-backdrop';
        document.body.appendChild(backdrop);
    }

    function closeSidebar() {
        sidebar && sidebar.classList.remove('open');
        backdrop.classList.remove('show');
    }
    function openSidebar() {
        sidebar && sidebar.classList.add('open');
        backdrop.classList.add('show');
    }

    if (toggle) {
        toggle.addEventListener('click', function (e) {
            e.stopPropagation();
            sidebar.classList.contains('open') ? closeSidebar() : openSidebar();
        });
    }
    backdrop.addEventListener('click', closeSidebar);
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeSidebar();
    });

    // Highlight active nav link by URL
    const here = window.location.pathname.toLowerCase().replace(/\/+$/, '') || '/';
    let bestMatch = null;
    let maxLen = -1;

    document.querySelectorAll('.sidebar-nav .nav-link').forEach(function (a) {
        const href = (a.getAttribute('href') || '').toLowerCase().replace(/\/+$/, '');
        if (!href) return;
        if (here === href || (href !== '/' && here.startsWith(href + '/'))) {
            if (href.length > maxLen) {
                maxLen = href.length;
                bestMatch = a;
            }
        }
    });

    if (bestMatch) {
        bestMatch.classList.add('active');
        bestMatch.setAttribute('aria-current', 'page');
    }

    // Auto-dismiss alerts after 5s
    document.querySelectorAll('.alert.alert-dismissible').forEach(function (el) {
        setTimeout(function () {
            el.style.transition = 'opacity .4s ease, transform .4s ease';
            el.style.opacity = '0';
            el.style.transform = 'translateY(-6px)';
            setTimeout(function () { el.remove(); }, 450);
        }, 5000);
    });

    // Apply Leave Modal Logic
    window.openApplyModal = function(e, url) {
        e && e.preventDefault();
        const content = document.getElementById('applyLeaveModalContent');
        if(!content) return;
        
        content.innerHTML = '<div class="p-5 text-center"><div class="spinner-border text-primary" role="status"></div></div>';
        
        const modal = new bootstrap.Modal(document.getElementById('applyLeaveModal'));
        modal.show();

        fetch(url || '/LeaveRequest/Apply', {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
        .then(r => r.text())
        .then(html => {
            content.innerHTML = html;
            if (window.$ && window.$.validator) {
                window.$.validator.unobtrusive.parse(content);
            }
        })
        .catch(err => {
            content.innerHTML = '<div class="p-4 text-danger">Failed to load form.</div>';
        });
    };

    window.submitApplyForm = function(e) {
        e.preventDefault();
        const form = e.target;
        
        if (window.$ && window.$(form).valid && !window.$(form).valid()) return;

        const btn = form.querySelector('button[type="submit"]');
        const originalText = btn.innerHTML;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Submitting...';
        btn.disabled = true;

        fetch(form.action, {
            method: 'POST',
            body: new FormData(form),
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
        .then(r => {
            const contentType = r.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                return r.json().then(data => {
                    if(data.success) {
                        window.location.reload();
                    }
                });
            } else {
                return r.text().then(html => {
                    const content = document.getElementById('applyLeaveModalContent');
                    content.innerHTML = html;
                    if (window.$ && window.$.validator) {
                        window.$.validator.unobtrusive.parse(content);
                    }
                });
            }
        });
    };
})();
