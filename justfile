build-go-librespot:
    cd go-librespot && go build ./cmd/daemon
    cp go-librespot/daemon Librespot.Gonet
