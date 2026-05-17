import sys

AZUL_NEON = "\033[38;5;39m"
GRIS_BORDE = "\033[38;5;240m"
GRIS_TEXTO = "\033[38;5;246m"
RESET = "\033[0m"

def mostrar_banner():
    logo_ascii = [
        "███    ███  ██       ██     ██████  ███████   ██████   █████   ███████  ██████ ",
        "████  ████  ██       ██     ██   ██ ██       ██       ██   ██  ██      ██    ██",
        "██ ████ ██  ██       ██     ██████  █████    ██   ███ ███████  ███████ ██    ██",
        "██  ██  ██  ██       ██     ██      ██       ██    ██ ██   ██       ██ ██    ██",
        "██      ██  ███████  ██     ██      ███████   ██████  ██   ██  ███████  ██████ "
    ]
    version_proyecto = "Mu Pegaso v1.0.0"
    url_proyecto = "https://mupegaso.com"
    server_info = "mu-pegaso-server, 0.1.0"
    deploy_info = "https://mupegaso.com"
    ANCHO_INTERNO = 86
    print(f"\n{GRIS_BORDE}┌{'─' * ANCHO_INTERNO}┐{RESET}")
    for linea in logo_ascii:
        espacios = (ANCHO_INTERNO - len(linea)) // 2
        margen_izq = " " * espacios
        margen_der = " " * (ANCHO_INTERNO - len(linea) - espacios)
        print(f"{GRIS_BORDE}│{RESET}{margen_izq}{AZUL_NEON}{linea}{RESET}{margen_der}{GRIS_BORDE}│{RESET}")
    print(f"{GRIS_BORDE}│{' ' * ANCHO_INTERNO}│{RESET}")
    print(f"{GRIS_BORDE}│{RESET}{AZUL_NEON}{version_proyecto.center(ANCHO_INTERNO)}{RESET}{GRIS_BORDE}│{RESET}")
    print(f"{GRIS_BORDE}│{RESET}{GRIS_TEXTO}{url_proyecto.center(ANCHO_INTERNO)}{RESET}{GRIS_BORDE}│{RESET}")
    print(f"{GRIS_BORDE}│{' ' * ANCHO_INTERNO}│{RESET}")
    print(f"{GRIS_BORDE}│{RESET}    💻 {AZUL_NEON}Server:{RESET}       {GRIS_TEXTO}{server_info:<62}{RESET} {GRIS_BORDE}│{RESET}")
    print(f"{GRIS_BORDE}│{RESET}    🚀 {AZUL_NEON}Deploy free:{RESET} {GRIS_TEXTO}{deploy_info:<62}{RESET} {GRIS_BORDE}│{RESET}")
    print(f"{GRIS_BORDE}└{'─' * ANCHO_INTERNO}┘{RESET}\n")

if __name__ == "__main__":
    mostrar_banner()
