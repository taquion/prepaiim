#!/bin/bash
set -e
echo "🚀 Deploying Portal IIM..."

DEPLOY_DIR="/var/www/prepaiim"
REPO_DIR="/root/.openclaw/workspace/prepaiim"

# --- Backend ---
echo ""
echo "📦 Backend..."
mkdir -p $DEPLOY_DIR/backend
rsync -av --exclude='__pycache__' --exclude='*.pyc' --exclude='.env' \
    $REPO_DIR/backend/ $DEPLOY_DIR/backend/

cd $DEPLOY_DIR

if [ ! -d "venv" ]; then
    python3 -m venv venv
fi

source venv/bin/activate
pip install -r backend/requirements.txt -q
echo "✅ Backend instalado"

# Setup DB (only if .env exists)
if [ -f "$DEPLOY_DIR/backend/.env" ]; then
    cd $DEPLOY_DIR/backend
    python scripts/seed.py
    cd $DEPLOY_DIR
else
    echo "⚠️  Crea $DEPLOY_DIR/backend/.env antes de correr el seed"
fi
deactivate

# --- Frontend ---
echo ""
echo "🎨 Frontend..."
mkdir -p $DEPLOY_DIR/frontend
rsync -av --exclude='node_modules' --exclude='dist' --exclude='.env' \
    $REPO_DIR/frontend/ $DEPLOY_DIR/frontend/

cd $DEPLOY_DIR/frontend

# Set API URL for production build
if [ -f "$DEPLOY_DIR/frontend/.env" ]; then
    source $DEPLOY_DIR/frontend/.env
fi

npm install -q
npm run build
echo "✅ Frontend compilado → $DEPLOY_DIR/frontend/dist"

# --- Nginx ---
echo ""
echo "🌐 Nginx..."
cp $REPO_DIR/nginx/iim.conf /etc/nginx/sites-available/prepaiim
ln -sf /etc/nginx/sites-available/prepaiim /etc/nginx/sites-enabled/prepaiim
nginx -t && systemctl reload nginx
echo "✅ Nginx configurado"

# --- Systemd ---
echo ""
echo "⚙️  Servicio..."
cp $REPO_DIR/scripts/prepaiim-backend.service /etc/systemd/system/
systemctl daemon-reload
systemctl enable prepaiim-backend
systemctl restart prepaiim-backend
echo "✅ Servicio iniciado"

echo ""
echo "🎉 Deploy completo!"
echo "   Frontend: http://$(hostname -I | awk '{print $1}')"
echo "   API docs: http://$(hostname -I | awk '{print $1}')/api/docs"
